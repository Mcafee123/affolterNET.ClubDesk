using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using affolterNET.ClubDesk.Core.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Group = affolterNET.ClubDesk.Core.Models.Group;

namespace affolterNET.ClubDesk.Core;

public class ClubdeskCsvReader
{
    private readonly string _dataPath;
    private readonly CsvConfiguration _cfg;

    private readonly string _birthdayRx =
        $"(?<{Group.Day}>[\\d]{{2,2}}).(?<{Group.Month}>[\\d]{{2,2}}).(?<{Group.Year}>[\\d]{{4,4}})";

    public ClubdeskCsvReader(string dataPath)
    {
        _dataPath = dataPath;
        _cfg = new CsvConfiguration(new CultureInfo("de-CH"))
        {
            HasHeaderRecord = false,
        };
    }

    public Dictionary<int, ParsedLists> ReadInvitationFiles(int startByYear)
    {
        Dictionary<int, ParsedLists> years = new();
        for (int i = startByYear; i <= DateTime.Now.Year; i++)
        {
            years.Add(i, ReadInvitationFile(i));
        }

        return years;
    }

    private ParsedLists ReadInvitationFile(int year)
    {
        var inputCsv = Path.Combine(_dataPath, $"Export-{year}.csv");
        using var reader = new StreamReader(inputCsv, Encoding.GetEncoding("iso-8859-1"));
        using var csv = new CsvReader(reader, _cfg);
        var records = csv.GetRecords<dynamic>().ToList();
        IDictionary<string, object> header = records.First();
        var result = new ParsedLists();
        const int skipFields = 4; // number of items before the invitation

        // Events
        foreach (var kvp in header.Skip(skipFields))
        {
            var t = new Event(kvp.Value.ToString() ?? string.Empty);
            result.Events.Add(t);
        }

        // Invitations
        foreach (IDictionary<string, object> row in records.Skip(1).Take(records.Count - 6))
        {
            if (row.Keys.Count - skipFields != result.Events.Count)
            {
                throw new InvalidOperationException("termin-invitation mismatch");
            }

            var first = row["Field1"].ToString()!;
            var last = row["Field2"].ToString()!;
            if (first == string.Empty && last == string.Empty)
            {
                Console.WriteLine("deleted or empty person-object, do not get attendance");
                continue;
            }

            var birthdayString = row["Field3"].ToString();
            var externalId = row["Field4"].ToString()!;
            if (string.IsNullOrWhiteSpace(externalId))
            {
                externalId = $"{first.ToLower()}.{last.ToLower()}.csvtool";
            }
            var person = new Person(first, last)
            {
                ExternalId = externalId
            };
            
            if (!string.IsNullOrWhiteSpace(birthdayString))
            {
                var ma = Regex.Match(birthdayString, _birthdayRx);
                if (ma.Success)
                {
                    person.Birthday = new DateTime(int.Parse(ma.Groups[Group.Year.ToString()].Value),
                        int.Parse(ma.Groups[Group.Month.ToString()].Value),
                        int.Parse(ma.Groups[Group.Day.ToString()].Value));
                }
            }


            result.Persons.Add(person);
            int idx = 0;
            foreach (var kvp in row.Skip(skipFields))
            {
                switch (kvp.Value)
                {
                    case "-":
                        break;
                    case "Ja":
                        result.Invitations.Add(new Invitation(Status.Ja, person.ExternalId, result.Events[idx].TrackingId));
                        break;
                    case "Nein":
                        result.Invitations.Add(new Invitation(Status.Nein, person.ExternalId, result.Events[idx].TrackingId));
                        break;
                    case "":
                        result.Invitations.Add(new Invitation(Status.None, person.ExternalId, result.Events[idx].TrackingId));
                        break;
                    default:
                        throw new InvalidOperationException("invalid invitation status");
                }

                idx++;
            }
        }

        // Summaries
        foreach (IDictionary<string, object> row in records.Skip(records.Count - 5))
        {
            var entries = row.Values.Skip(skipFields).Cast<string>().Select(int.Parse).ToList();
            var label = row["Field1"].ToString()!;
            switch (label)
            {
                case "Eingeladen":
                    Set(entries, (i, count) => result.Events[i].Invited = count);
                    break;
                case "Ja":
                    Set(entries, (i, count) => result.Events[i].Yes = count);
                    break;
                case "Nein":
                    Set(entries, (i, count) => result.Events[i].No = count);
                    break;
                case "Vielleicht":
                    Set(entries, (i, count) => result.Events[i].MayBe = count);
                    break;
                case "Keine Antwort":
                    Set(entries, (i, count) => result.Events[i].None = count);
                    break;
                default:
                    throw new InvalidOperationException("invalid invitation status");
            }
        }

        return result;
    }

    private void Set(List<int> entries, Action<int, int> setter)
    {
        for (var i = 0; i < entries.Count; i++)
        {
            setter(i, entries[i]);
        }
    }
}