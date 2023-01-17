namespace affolterNET.ClubDesk.Core;

public class UpdateWorker
{
    private readonly string _dataPath;
    private readonly ClubdeskCsvReader _reader;
    private readonly DbWriter _writer;
    private readonly AttendanceDownloader _downloader;

    public UpdateWorker(string scraperPath, string email, string pw, string dataPath)
    {
        _dataPath = dataPath;
        _downloader = new AttendanceDownloader(scraperPath, email, pw);
        _reader = new ClubdeskCsvReader(_dataPath);
        _writer = new DbWriter(_dataPath);
    }

    public async Task DownloadPersons(string outputFile)
    {
        var retries = -1;
        var renamedOutput = Path.Combine(_dataPath, $"Export-Persons.csv");
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
                if (File.Exists(renamedOutput))
                {
                    File.Delete(renamedOutput);
                }
    
                File.Move(outputFile, renamedOutput);
                return;
            }
        }
        throw new InvalidOperationException(
            $"output file \"{renamedOutput}\" not created - error occurred. see video");
    }

    public async Task DownloadEventsAndInvitations(string outputFile)
    {
        var currentYear = DateTime.Now.Year;
        var y = 2019;
        var retries = -1;
        while (y <= currentYear)
        {
            var renamedOutput = Path.Combine(_dataPath, $"Export-{y}.csv");
            if (File.Exists(renamedOutput) && y != currentYear)
            {
                y++;
                continue;
            }

            var result = _downloader.GetAttendance(y);
            Console.WriteLine(result);
            var i = 0;
            while (!File.Exists(outputFile) && i < 20)
            {
                await Task.Delay(200);
                i++;
            }

            if (!File.Exists(outputFile))
            {
                retries++;
                if (retries < 3)
                {
                    Console.WriteLine("Error - Retry");
                }
                else
                {
                    throw new InvalidOperationException(
                        $"output file \"{renamedOutput}\" not created - error occurred. see video");
                }
            }
            else
            {
                if (File.Exists(renamedOutput))
                {
                    File.Delete(renamedOutput);
                }

                File.Move(outputFile, renamedOutput);
                retries = 0;
                y++;
            }
        }
    }
    
    public async Task UpdateDb(int? startByYear = null)
    {
        // if a year is set, insert that and all following years,
        // otherwise just import the current year
        if (startByYear == null)
        {
            startByYear = DateTime.Now.Year;
        }

        var years = _reader.ReadInvitationFiles(startByYear.Value);
        await _writer.Update(years);
    }
}