#!/bin/bash

# author: martin@affolter.net

include_file="__config.sh"
if test -f "$include_file"; then
  . "$include_file"
  echo "email & pw loaded from: $include_file"
fi

echo "Download Calendar File"
curl -o ics/wildstars.ics  -H "Accept:text/calendar" "$icsurl"