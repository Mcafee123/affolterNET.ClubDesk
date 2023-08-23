#!/bin/bash

# author: martin@affolter.net

containername="clubdesk"
tagname="0.1.0"

stop_container() {
  c="$1"
  echo "check stopping $c ..."
  running=$(docker ps | grep $c)
  if [ "$running" == "" ]; then
    stopped=$(docker ps --all | grep $c)
    if [ "$stopped" == "" ]; then
      echo "CONTAINER DOES NOT EXIST"
      return
    fi
  else
    echo "STOP $c"
    docker container stop "$c"
  fi
  echo "REMOVE $c"
  docker rm "$c"
}
