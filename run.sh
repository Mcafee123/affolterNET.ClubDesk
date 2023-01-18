#!/bin/bash

# author: martin@affolter.net

export email="ADD E-MAIL"
export pw="ADD-PW"
export projectroot="../"
export groups="Spiel 1. Mannschaft, Spiel 2. Mannschaft, Spiel 1. & 2. Mannschaft,Training 1. Mannschaft, Training 2. Mannschaft, Training 1. & 2. Mannschaft, Sitzung, Allgemein, Spiel Senioren, Training Senioren, Anlass EHC, Anlass SCO, Cupspiel"
export download="true"

include_file="__config.sh"
if test -f "$include_file"; then
  . "$include_file"
  echo "email & pw loaded from: $include_file"
fi

pushd .
cd console
./affolterNET.ClubDesk.Terminal
popd