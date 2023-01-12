using affolterNET.ClubDesk.Core;
using Microsoft.Extensions.Configuration;

// config
var builder = new ConfigurationBuilder();
var config = builder
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();
var email = config.GetValue<string>("email");
ArgumentNullException.ThrowIfNull(email);

var pw = config.GetValue<string>("pw");
ArgumentNullException.ThrowIfNull(pw);

// scraper tool
var scraperPath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "scraper").Normalize();
if (!Directory.Exists(scraperPath))
{
    throw new DirectoryNotFoundException($"Scraper-Path \"{scraperPath}\" not found!");
}
var dataPath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "data").Normalize();
if (!Directory.Exists(dataPath))
{
    throw new DirectoryNotFoundException($"Data-Path \"{dataPath}\" not found!");
}

var outputPath = Path.Combine(scraperPath, "cypress", "downloads");
var outputFile = Path.Combine(outputPath, "Export.csv");

// downloader process starter
var downloader = new AttendanceDownloader(scraperPath, email, pw);
var currentYear = DateTime.Now.Year;
var y = 2019;
var retries = -1;
while (y <= currentYear)
{
    var renamedOutput = Path.Combine(dataPath, $"Export-{y}.csv");
    if (File.Exists(renamedOutput) && y != currentYear)
    {
        y++;
        continue;
    }

    var result = downloader.GetCsv(y);
    Console.WriteLine(result);
    if (!File.Exists(outputFile))
    {
        retries++;
        if (retries < 3)
        {
            Console.WriteLine("Error - Retry");
        }
        else
        {
            throw new InvalidOperationException($"output file \"{renamedOutput}\" not created - error occurred. see video");
        }
    }
    else
    {
        File.Move(outputFile, renamedOutput);
        retries = 0;
        y++;
    }
}
