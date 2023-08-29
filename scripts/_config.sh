#!/bin/bash

# author: martin@affolter.net

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
echo "DIR: $DIR"

env=$1

if [ -z "$env" ]; then
    echo "environment not set"
    exit 99
fi

. $DIR/_func.sh

tenant_id="9c0f6304-c41a-4891-8379-ed3cbfc54535"
subscription="affolter.NET MPN"
# devops settings
proj="Clubdesk"
lower_basename=$(echo $(tr '[:upper:]' '[:lower:]' <<< "$proj"))
basename=${lower_basename/./-}
settingsgroup="${basename}-settings"
connection_name="$proj Connection"
location="westeurope"

instance="https://login.microsoftonline.com/"
domain="formsg.ch"
SENDGRID_FROM_EMAIL="donotreply@affolter.net"
SENDGRID_FROM_NAME="Clubdesk Cloud Service"

application_url="admin.formsg.ch"
branch_name="main"
github_user="Mcafee123"
github_repo="formsg"
static_site_repo="https://github.com/$github_user/$github_repo"

export GITHUB_USER_NAME="$github_user"

# storage
storageAccountName="${basename/-/}${env}stor"

# function app name
functionsAppName="$basename-$env-fnc"
functionsAppUrl="https://$functionsAppName.azurewebsites.net"
functionsAppDir="../svc"

swaAppName="$basename-$env-swa"
logAnalyticsWorkspace="$basename-$env-law"

# rg name
resourceGroup="rg_${basename/-/_}_$env"

# template blob container
blobtemplatecontainer="formtemplates"

# registry name
registryname="${basename/-/}${env}reg"
reguri="${registryname}.azurecr.io"

## PDF container
containerpdfenvname="${basename/-/}${env}pdfenv"
containerpdfappname="${basename/-/}${env}pdf"
pdfimagename="md2pdf"
pdftag="v1.0.7"

# key vault name
keyvaultname="${basename/-/}${env}kv"
keyvaultprincipal="sp_${keyvaultname}"
