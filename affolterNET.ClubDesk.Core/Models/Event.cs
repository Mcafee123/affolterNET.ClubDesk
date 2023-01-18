using System.Text.RegularExpressions;

namespace affolterNET.ClubDesk.Core.Models;

public class Event
{
    public Event()
    {
        TrackingId = Guid.NewGuid().ToString();
    }

    public Event(string csvInput): this()
    {
        if (csvInput == string.Empty)
        {
            throw new InvalidOperationException("termin data empty");
        }

        var lines = csvInput.Split("\r\n");
        if (lines.Length != 2)
        {
            throw new InvalidOperationException("termin line count mismatch");
        }

        Subject = lines[0];
        var dateMatch = Regex.Match(lines[1].Substring(3),
            $"(?<{Group.Day}>[\\d]{{2,2}}).(?<{Group.Month}>[\\d]{{2,2}}).(?<{Group.Year}>[\\d]{{4,4}})[\\s](?<{Group.StartHours}>[\\d]{{2,2}}).(?<{Group.StartMinutes}>[\\d]{{2,2}})[\\s]-[\\s](?<{Group.EndHours}>[\\d]{{2,2}}).(?<{Group.EndMinutes}>[\\d]{{2,2}})");
        if (!dateMatch.Success)
        {
            throw new InvalidOperationException("could not read date or time");
        }

        Start = dateMatch.GetStart();
        End = dateMatch.GetEnd();
    }

    public int EventId { get; set; }
    public int EventTypeId { get; set; }
    public string TrackingId { get; private set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Subject { get; set; }
    public int Invited { get; set; }
    public int Yes { get; set; }
    public int No { get; set; }
    public int None { get; set; }
    public int MayBe { get; set; }
    public string? EventType { get; set; }
}

public enum Group
{
    Day,
    Month,
    Year,
    StartHours,
    StartMinutes,
    EndHours,
    EndMinutes
}

public static class IntParser
{
    public static DateTime GetStart(this Match dateMatch)
    {
        return new DateTime(dateMatch.Get(Group.Year), dateMatch.Get(Group.Month), dateMatch.Get(Group.Day),
            dateMatch.Get(Group.StartHours), dateMatch.Get(Group.StartMinutes), 0);
    }

    public static DateTime GetEnd(this Match dateMatch)
    {
        return new DateTime(dateMatch.Get(Group.Year), dateMatch.Get(Group.Month), dateMatch.Get(Group.Day),
            dateMatch.Get(Group.EndHours), dateMatch.Get(Group.EndMinutes), 0);
    }

    public static int Get(this Match dateMatch, Group groupName)
    {
        return Convert.ToInt32(dateMatch.Groups[groupName.ToString()].Value);
    }
}