#!/bin/bash

# author: martin@affolter.net

version="0.7.0"
informationalVersion="$version"

pushd .

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
echo "DIR: $DIR"
cd "$DIR"

cd scraper
npm install
cd ..

outputdir="console"
echo "exists: $outputdir?"
[ -d "$outputdir" ] && rm -r $outputdir

dockerputdir="docker/clubdesk/console"
echo "exists: $dockerputdir?"
[ -d "$dockerputdir" ] && rm -r $dockerputdir

cd affolterNET.ClubDesk.Terminal
dotnet publish -o ../$outputdir --configuration Release -p:PublishSingleFile=true -p:Version="$version" -p:InformationalVersion="$informationalVersion" --self-contained true
dotnet publish -o ../$dockerputdir --configuration Release -r linux-x64 -p:PublishSingleFile=true -p:Version="$version" -p:InformationalVersion="$informationalVersion" --self-contained true

popd
