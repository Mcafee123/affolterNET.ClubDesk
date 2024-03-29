﻿using affolterNET.ClubDesk.Core;
using Microsoft.Extensions.Configuration;

// config
var builder = new ConfigurationBuilder();
var config = builder
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();
var email = config.GetValue<string>("email");
ArgumentNullException.ThrowIfNull(email);

var pw = config.GetValue<string>("pw");
ArgumentNullException.ThrowIfNull(pw);

var groups = config.GetValue<string?>("groups");
if (string.IsNullOrWhiteSpace(groups))
{
    groups =
        "Spiel 1. Mannschaft, Spiel 2. Mannschaft, Spiel 1. & 2. Mannschaft,Training 1. Mannschaft, Training 2. Mannschaft, Training 1. & 2. Mannschaft, Sitzung, Allgemein, Spiel Senioren, Training Senioren, Anlass EHC, Anlass SCO, Cupspiel";
}

var download = config.GetValue<bool?>("download") ?? false;
var relativeProjectRoot = config.GetValue<string>("projectroot");
if (string.IsNullOrWhiteSpace(relativeProjectRoot))
{
    relativeProjectRoot = Path.Combine("..", "..", "..", "..");
}

// var env = Environment.GetEnvironmentVariables();
// foreach (var key in env.Keys)
// {
//     Console.WriteLine("{0}: {1}", key, env[key]);
// }

// scraper tool
var scraperPath = Path.Combine(Environment.CurrentDirectory, relativeProjectRoot, "scraper").Normalize();
if (!Directory.Exists(scraperPath))
{
    throw new DirectoryNotFoundException($"Scraper-Path \"{scraperPath}\" not found!");
}

var dataPath = Path.Combine(Environment.CurrentDirectory, relativeProjectRoot, "data").Normalize();
if (!Directory.Exists(dataPath))
{
    throw new DirectoryNotFoundException($"Data-Path \"{dataPath}\" not found!");
}

var outputPath = Path.Combine(scraperPath, "cypress", "downloads");


// update db
var updater = new UpdateWorker(scraperPath, email, pw, dataPath, groups);
// download persons
var personsOutputFile = Path.Combine(outputPath, "export.csv");
if (download)
{
    await updater.DownloadPersons(personsOutputFile);
}

// update db
var pers = await updater.UpdatePersons();

// download events and invitations
var eventsOutputFile = Path.Combine(outputPath, "Export.csv");
if (download)
{
    await updater.DownloadEventsAndInvitations(eventsOutputFile);
}

// update db
await updater.UpdateDb(pers, 2019);
