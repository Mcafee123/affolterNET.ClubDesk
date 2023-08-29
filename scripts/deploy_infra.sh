#!/bin/bash

# author: martin@affolter.net

# application insights: az extension add -n application-insights
# brew tap azure/functions
# brew install azure-functions-core-tools@3

# DEPLOY INFRASTRUCTURE FOR FORMSG ADMIN-SWA AND FORMSG CORE SERVICE

env=${1:-prod}

. _config.sh $env

# az_login
set_subscription

# create the resource group
az group create -n "$resourceGroup" -l "$location"

# to see if anything was already in there
az resource list -g $resourceGroup -o tsv

# create key vault
az keyvault create --name "$keyvaultname" --resource-group "$resourceGroup" --location "$stor_location"

# create service principal for keyvault
create_serviceprincipal "Contributor" "$keyvaultprincipal" "/subscriptions/$subscriptionid/resourceGroups/$resourceGroup/providers/Microsoft.KeyVault/vaults/$keyvaultname"

# create a storage account
az storage account create \
  -n "$storageAccountName" \
  -l "$stor_location" \
  -g "$resourceGroup" \
  --sku Standard_LRS

storageAccountKey=$(az storage account keys list -n $storageAccountName --query [0].value -o tsv)

echo
echo "create __stor.sh"
echo "export STORAGE_ACCOUNT_NAME=\"$storageAccountName\"" > __${env}_stor.sh
echo "export STORAGE_ACCOUNT_KEY"=\"$storageAccountKey\" >> __${env}_stor.sh
echo

storagecn="DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net;AccountName=$storageAccountName;AccountKey=$storageAccountKey"
az keyvault secret set \
  --name "FORMSG-STORAGE" \
  --vault-name "$keyvaultname" \
  --content-type "connection string" \
  --value "$storagecn"

# create log analytics workspace
lawWorkspaceId=$(az monitor log-analytics workspace create \
  -g "$resourceGroup" \
  -n "$logAnalyticsWorkspace"\
  --query customerId \
  -o tsv)

loganalyticsworkspacekey=$(az monitor log-analytics workspace get-shared-keys -n "$logAnalyticsWorkspace" -g "$resourceGroup" --query primarySharedKey -o tsv)
echo
echo "create __${env}_law.sh"
echo "export LOG_ANALYTICS_WORKSPACE_ID=\"$lawWorkspaceId\"" > __${env}_law.sh
echo "export LOG_ANALYTICS_WORKSPACE_KEY=\"$loganalyticsworkspacekey\"" >> __${env}_law.sh
echo

# create container registry
az acr create \
  -g "$resourceGroup" \
  --name "$registryname" \
  --sku Basic \
  --admin-enabled true

acrpassword=$(az acr credential show -n "$registryname" --query passwords[0].value -o tsv)

echo
echo "create __${env}_reg.sh"
echo "export ACI_ADMIN_PASSWORD=\"$acrpassword\"" > __${env}_reg.sh
echo

# create an app insights instance
appInsightsName="$basename-insights"
instrumentationKey=$(az monitor app-insights component create \
  --app "$appInsightsName" \
  --location "$location" \
  -g "$resourceGroup" \
  --workspace "$logAnalyticsWorkspace" \
  --kind web \
  --application-type web \
  --query instrumentationKey \
  -o tsv)

echo
echo "create __${env}_insights.sh"
echo "export APPINSIGHTS_INSTRUMENTATIONKEY=\"$instrumentationKey\"" > __${env}_insights.sh
echo

# swa
url="$(az staticwebapp create \
  --branch "$branch_name" \
  --location "$location" \
  --name "$swaAppName" \
  --resource-group "$resourceGroup" \
  --source "$static_site_repo" \
  --sku Free \
  --login-with-github \
  --query defaultHostname)"

hostnames=$(az staticwebapp hostname list \
  -n "$swaAppName" \
  -g "$resourceGroup" \
  --query [0].name \
  -o tsv)

if [ -z $hostnames ]; then
  # add custom domain name
  az staticwebapp hostname set  -n "$swaAppName" --hostname "$application_url"
  echo ""
  echo "create CNAME entry '$application_url' for $url"
  echo "create TXT entry '' for 'asuid.$url' with value from ERROR (above)"
  echo ""
  echo "press <Enter> to continue"
  echo ""
  read
  # add custom domain name
  az staticwebapp hostname set  -n "$swaAppName" --hostname "$application_url"
fi

# create svc function app
az functionapp create \
  -n $functionsAppName \
  -g $resourceGroup \
  --storage-account $storageAccountName \
  --consumption-plan-location "$stor_location" \
  --app-insights $appInsightsName \
  --os-type Linux \
  --runtime dotnet \
  --functions-version 4

# create a managed identity (idempotent - returns the existing identity if there already is one)
funcAppManagedIdentity="$(az functionapp identity assign -n "$functionsAppName" -g "$resourceGroup" --query principalId -o tsv)"
echo "funcAppManagedIdentity: $funcAppManagedIdentity"

# create access policy for func app in key vault
az keyvault set-policy -n $keyvaultname -g $resourceGroup \
            --object-id $funcAppManagedIdentity --secret-permissions get set list

# create access policy for swa in key vault
az keyvault set-policy -n $keyvaultname -g $resourceGroup \
            --spn $SP_DEVOPS_ID --secret-permissions get set list
