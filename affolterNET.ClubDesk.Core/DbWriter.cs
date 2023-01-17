using affolterNET.ClubDesk.Core.Models;

namespace affolterNET.ClubDesk.Core;

public class DbWriter
{
    private readonly Database _db;

    public DbWriter(string dataPath)
    {
        _db = new Database(Path.Combine(dataPath, "clubdesk.sqlite"));
    }
    
    public async Task Update(Dictionary<int, ParsedLists> years)
    {
        await _db.DeleteEventsAndInvitationsOfYears(years.Keys.ToList());
        var persons = await _db.UpdatePersons(years);
        var events = await _db.UpdateEvents(years);
        await _db.UpdateInvitations(years, persons, events);
    }
}