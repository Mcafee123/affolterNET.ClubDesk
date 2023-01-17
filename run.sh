#!/bin/bash

# author: martin@affolter.net

export email="ADD E-MAIL"
export pw="ADD-PW"
export projectroot="../"

include_file="__config.sh"
if test -f "$include_file"; then
  . "$include_file"
  echo "email & pw loaded from: $include_file"
fi

pushd .
cd console
./affolterNET.ClubDesk.Terminal
popd