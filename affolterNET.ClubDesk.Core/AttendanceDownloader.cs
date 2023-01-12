using System.Diagnostics;

namespace affolterNET.ClubDesk.Core;

public class AttendanceDownloader
{
    private readonly string _scraperPath;
    private readonly string _email;
    private readonly string _pw;

    public AttendanceDownloader(string scraperPath, string email, string pw)
    {
        _scraperPath = scraperPath;
        _email = email;
        _pw = pw;
    }

    public string GetCsv(int year)
    {
        var flags = $"{{\"\"email\"\":\"\"{_email}\"\",\"\"pw\"\":\"\"{_pw}\"\",\"\"year\"\":\"\"{year}\"\" }}";
        var process = new Process();
        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            WorkingDirectory = _scraperPath,
            WindowStyle = ProcessWindowStyle.Hidden,
            FileName = "npx",
            Arguments = $"cypress run --env flags=\"{flags}\" --spec \"cypress/e2e/clubdesk.cy.js\"",
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };
        process.StartInfo = startInfo;
        process.Start();

        var cypressProcessOutput = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return cypressProcessOutput;
    }
}