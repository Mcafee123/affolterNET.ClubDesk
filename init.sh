#!/bin/bash

# author: martin@affolter.net

version="0.7.0"
informationalVersion="$version"

pushd .
cd scraper
npm install
popd

outputdir="console"
echo "exists: $outputdir?"
[ -d "$outputdir" ] && rm -r $outputdir

pushd .
cd affolterNET.ClubDesk.Terminal
dotnet publish -o ../$outputdir --configuration Release -p:PublishSingleFile=true -p:Version="$version" -p:InformationalVersion="$informationalVersion" --self-contained true
popd
