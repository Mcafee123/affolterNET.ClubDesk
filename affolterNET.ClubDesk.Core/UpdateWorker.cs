using affolterNET.ClubDesk.Core.Models;

namespace affolterNET.ClubDesk.Core;

public class UpdateWorker
{
    private readonly string _dataPath;
    private readonly ClubdeskCsvReader _reader;
    private readonly DbWriter _writer;
    private readonly AttendanceDownloader _downloader;
    private readonly string _personsFile;
    private readonly List<string> _groups;

    public UpdateWorker(string scraperPath, string email, string pw, string dataPath, string groups)
    {
        _dataPath = dataPath;
        _personsFile = Path.Combine(_dataPath, "Export-Persons.csv");
        _downloader = new AttendanceDownloader(scraperPath, email, pw);
        _reader = new ClubdeskCsvReader(_dataPath);
        _writer = new DbWriter(_dataPath);
        _groups = groups.Split(",").Select(g => g.Trim()).ToList();
    }

    public async Task DownloadPersons(string outputFile)
    {
        var retries = -1;
        while (retries < 3)
        {
            var result = _downloader.GetPersons();
            Console.WriteLine(result);
            var i = 0;
            while (!File.Exists(outputFile) && i < 5)
            {
                await Task.Delay(200);
                i++;
            }
            
            if (!File.Exists(outputFile))
            {
                retries++;
                Console.WriteLine("Error - Retry");
            }
            else
            {
                if (File.Exists(_personsFile))
                {
                    File.Delete(_personsFile);
                }
    
                File.Move(outputFile, _personsFile);
                return;
            }
        }
        throw new InvalidOperationException(
            $"output file \"{_personsFile}\" not created - error occurred. see video");
    }
    
    public async Task<List<Person>> UpdatePersons()
    {
        var pers = _reader.ReadPersons(_personsFile);
        return await _writer.UpdatePersons(pers);
    }

    public void DownloadEventsAndInvitations(string outputFile)
    {
        var currentYear = DateTime.Now.Year;
        for (var y = 2019; y <= currentYear; y++)
        {
            foreach (var grp in _groups)
            {
                const int maxRetries = 3;
                for (var retries = 1; retries <= maxRetries; retries++)
                {
                    Console.WriteLine("Download {0}: \"{1}\", {2}", retries, grp, y);
                    var groupName = _reader.SanitizeGroupName(grp);
                    var filename = string.Format("Export-{0}-{1}.csv", y, groupName);
                    var videoDestPath = Path.Combine(_dataPath, "videos", y.ToString(), groupName);
                    var renamedOutput = Path.Combine(_dataPath, filename);
                    if (File.Exists(renamedOutput) && y != currentYear)
                    {
                        Console.WriteLine("Exists \"{0}\", {1}", grp, y);
                        DeleteVideos(videoDestPath);
                        break;
                    }

                    var result = _downloader.GetAttendance(y, grp);
                    if (!File.Exists(outputFile))
                    {
                        if (retries < maxRetries)
                        {
                            Console.WriteLine(result);
                            Console.WriteLine("Error - Retry {0}: \"{1}\", {2}", retries, grp, y);
                            CopyVideo(outputFile, videoDestPath, "invitations.cy.js.mp4");
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                $"output file \"{renamedOutput}\" not created - error occurred. see video");
                        }
                    }
                    else
                    {
                        Console.WriteLine("OK {0}: \"{1}\", {2}", retries, grp, y);
                        if (File.Exists(renamedOutput))
                        {
                            File.Delete(renamedOutput);
                        }

                        File.Move(outputFile, renamedOutput);
                        DeleteVideos(videoDestPath);
                        break;
                    }
                }
            }
        }
    }

    public async Task UpdateDb(List<Person> persons, int? startByYear = null)
    {
        // if a year is set, insert that and all following years,
        // otherwise just import the current year
        if (startByYear == null)
        {
            startByYear = DateTime.Now.Year;
        }

        var years = _reader.ReadInvitationFiles(startByYear.Value, _groups);
        await _writer.UpdateEventsAndInvitations(years, persons);
    }

    private void CopyVideo(string outputFile, string videoDestPath, string videoFileName)
    {
        var outputPath = Path.GetDirectoryName(outputFile)!;
        var videoFile = Path.Combine(outputPath, "..", "videos", videoFileName);
        var d = DateTime.Now;
        var videoDestFileName = $"{d.Year}-{d.Month}-{d.Day} {d.Hour}_{d.Minute}_{d.Second}.mp4";
        if (!Directory.Exists(videoDestPath))
        {
            Directory.CreateDirectory(videoDestPath);
        }
        File.Copy(videoFile, Path.Combine(videoDestPath, videoDestFileName));
    }
    
    private void DeleteVideos(string videoDestPath)
    {
        if (Directory.Exists(videoDestPath))
        {
            Directory.Delete(videoDestPath, true);
        }

        var parentPath = Directory.GetParent(videoDestPath);
        if (parentPath != null && parentPath.Exists && parentPath.GetDirectories().Length == 0)
        {
            parentPath.Delete();
        }
    }
}