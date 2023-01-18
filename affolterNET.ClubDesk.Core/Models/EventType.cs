namespace affolterNET.ClubDesk.Core.Models;

public class EventType
{
    public EventType()
    {
        
    }
    
    public EventType(int id, string description)
    {
        EventTypeId = id;
        Description = description;
    }
    
    public int EventTypeId { get; set; }
    public string Description { get; set; } = null!;
}