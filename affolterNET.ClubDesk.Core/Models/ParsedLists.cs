namespace affolterNET.ClubDesk.Core.Models;

public class ParsedLists
{
    public List<Person> Persons { get; set; } = new();
    public List<Event> Events { get; set; } = new();
    public List<Invitation> Invitations { get; set; } = new();
}