#!/bin/bash

# author: martin@affolter.net
action=${1:-"open"}
echo "action: $action"

include_file="../__config.sh"
if test -f "$include_file"; then
  . "$include_file"
  echo "email & pw loaded from: $include_file"
else
  echo "USER AND PW MISSING"
  exit 1
fi
# echo '{\"email\":\"'$email'\",\"pw\":\"'$pw'\",\"year\":\"2022\"}'
# echo $"email: $email"
# echo "pw: $pw"
if [ $action == "run" ]; then
  cypress run --env flags="{\"email\":\"$email\",\"pw\":\"$pw\",\"year\":\"2022\"}" --spec 'cypress/e2e/persons.cy.js'
else
  cypress open --env flags="{\"email\":\"$email\",\"pw\":\"$pw\",\"year\":\"2019\",\"group\":\"Training 1. & 2. Mannschaft\"}"
fi