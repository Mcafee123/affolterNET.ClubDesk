using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using affolterNET.ClubDesk.Core.Converters;
using affolterNET.ClubDesk.Core.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Group = affolterNET.ClubDesk.Core.Models.Group;

namespace affolterNET.ClubDesk.Core;

public class ClubdeskCsvReader
{
    private readonly string _dataPath;
    private readonly CsvConfiguration _cfg;
    private readonly Regex _groupNameRx = new (@"[^a-zA-Z1-9\\-_]");

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

    public List<Person> ReadPersons(string personsFile)
    {
        var cfg = new CsvConfiguration(new CultureInfo("de-CH"))
        {
            HasHeaderRecord = true,
        };
        using var reader = new StreamReader(personsFile, Encoding.GetEncoding("iso-8859-1"));
        using var csv = new CsvReader(reader, cfg);
        var list = csv.GetRecords<Person>().ToList();
        return list;
    }

    public Dictionary<int, ParsedLists> ReadInvitationFiles(int startByYear, List<string> groups)
    {
        Dictionary<int, ParsedLists> years = new();
        for (int year = startByYear; year <= DateTime.Now.Year; year++)
        {
            var lists = new ParsedLists();
            var di = new DirectoryInfo(_dataPath);
            foreach (var file in di.GetFiles($"Export-{year}-*.csv"))
            {
                var eventType = GetEventType(file.FullName, groups);
                var r = ReadInvitationFile(file.FullName, eventType);
                lists.Invitations.AddRange(r.Invitations);
                lists.Events.AddRange(r.Events);
            }
            years.Add(year, lists);
        }

        return years;
    }

    private string GetEventType(string file, List<string> groups)
    {
        var grpName = Path.GetFileNameWithoutExtension(file).Substring(12);
        foreach (var g in groups)
        {
            if (SanitizeGroupName(g) == grpName)
            {
                return g;
            }
        }
        throw new InvalidOperationException($"eventtype \"{grpName}\" not in list");
    }

    public string SanitizeGroupName(string grp)
    {
        return _groupNameRx.Replace(grp, "_");
    }

    private ParsedLists ReadInvitationFile(string inputCsv, string eventType)
    {
        try
        {
            using var reader = new StreamReader(inputCsv, Encoding.GetEncoding("iso-8859-1"));
            using var csv = new CsvReader(reader, _cfg);
            var records = csv.GetRecords<dynamic>().ToList();
            var result = new ParsedLists();
            if (records.Count < 1)
            {
                return result;
            }

            IDictionary<string, object> header = records.First();
            const int skipFields = 4; // number of items before the invitation

            // Events
            foreach (var kvp in header.Skip(skipFields))
            {
                var t = new Event(kvp.Value.ToString() ?? string.Empty)
                {
                    EventType = eventType
                };
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
                var birthday = new CdDateOnlyConverter().ConvertFromString(birthdayString, null!, null!) as DateTime?;
                var externalId = row["Field4"].ToString()!;

                int idx = 0;
                foreach (var kvp in row.Skip(skipFields))
                {
                    var inv = new Invitation(Status.None, result.Events[idx].TrackingId, first, last, birthday,
                        externalId);
                    if (string.IsNullOrEmpty(kvp.Value.ToString()))
                    {
                        // ok already as inv was initialized with Status.None
                        result.Invitations.Add(inv);
                    }
                    else
                    {
                        var status = kvp.Value.ToString()!;
                        if (status == "-")
                        {
                            // person not invited to this event
                        }
                        else if (status == "Ja" || status.StartsWith("Ja "))
                        {
                            inv.StatusId = (int)Status.Ja;
                            result.Invitations.Add(inv);
                        }
                        else if (status == "Nein" || status.StartsWith("Nein "))
                        {
                            inv.StatusId = (int)Status.Nein;
                            result.Invitations.Add(inv);
                        }
                        else
                        {
                            throw new InvalidOperationException("invalid invitation status");
                        }
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
        catch
        {
            Console.WriteLine($"Exception in {inputCsv} ({eventType})");
            throw;
        }
    }

    private void Set(List<int> entries, Action<int, int> setter)
    {
        for (var i = 0; i < entries.Count; i++)
        {
            setter(i, entries[i]);
        }
    }
}