# Readme.md

## How it works

The C# console application **_affolterNET.ClubDesk.Terminal_** starts a cypress test, which is located in the **_scraper_**-folder to download all calendar events with the registrations of people. There is one file downloaded per year, that will be placed in the **_data_**-folder.

## How to run it

* run _init.sh_
* set environment variables (see below)
* run _run.sh_

## Environment variables to set

* **email**: User-E-Mail
* **pw**: User-Password
* **projectroot**: Relative Path from the executable to the folder with "affolterNET.ClubDesk.Terminal", "data", "scraper", etc.