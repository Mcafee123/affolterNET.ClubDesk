#!/bin/sh

# author: martin@affolter.net

echo "build"

rm -rf ./scraper && mkdir -p ./scraper && cp -R ../../scraper/* ./scraper/
rm -rf ./run.sh && cp ../../run.sh ./run.sh
rm -rf ./__config.sh && cp ../../__config.sh ./__config.sh

docker build -t $containername:$tagname .