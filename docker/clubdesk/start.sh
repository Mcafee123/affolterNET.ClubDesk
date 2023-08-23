#!/bin/sh

# author: martin@affolter.net

. _config.sh
stop_container $containername

. ../../init.sh

. _build.sh

# apifolder="$(pwd)/../../api"
# if test -f "$apifolder"; then
#   echo "API-Folder \"$apifolder\" does not exist"
# fi
# appfolder="$(pwd)/../../app"
# if test -f "$appfolder"; then
#   echo "APP-Folder \"$appfolder\" does not exist"
# fi
datafolder="$(pwd)/../../data"
if test -f "$datafolder"; then
  echo "DATA-Folder \"$datafolder\" does not exist"
fi

# apivolume="$apifolder:/src/api"
# appvolume="$appfolder:/src/app"
datavolume="$datafolder:/src/data"
echo "run"

docker run -it -v "$datavolume" --name $containername $containername:$tagname

