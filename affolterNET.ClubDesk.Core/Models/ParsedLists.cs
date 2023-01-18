namespace affolterNET.ClubDesk.Core.Models;

public class ParsedLists
{
    public List<Event> Events { get; set; } = new();
    public List<Invitation> Invitations { get; set; } = new();
}