using affolterNET.ClubDesk.Core.Models;

namespace affolterNET.ClubDesk.Core;

public class DbWriter
{
    private readonly Database _db;

    public DbWriter(string dataPath)
    {
        _db = new Database(Path.Combine(dataPath, "clubdesk.sqlite"));
    }

    public async Task<List<Person>> UpdatePersons(List<Person> persons)
    {
        return await _db.UpdatePersons(persons);
    }

    public async Task UpdateEventsAndInvitations(Dictionary<int, ParsedLists> years, List<Person> persons)
    {
        await _db.DeleteEventsAndInvitationsOfYears(years.Keys.ToList());
        var events = await _db.UpdateEvents(years);
        await _db.UpdateInvitations(years, persons, events);
    }
}