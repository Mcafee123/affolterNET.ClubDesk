namespace affolterNET.ClubDesk.Core.Models;

public class Invitation
{
    public Invitation()
    {
    }

    public Invitation(Status status, string personExternalId, string eventTrackingId)
    {
        PersonExternalId = personExternalId;
        EventTrackingId = eventTrackingId;
        StatusId = (int)status;
    }

    public int EventId { get; set; }
    public int PersonId { get; set; }
    public int StatusId { get; set; }

    public Status Status => (Status)StatusId;
    public string PersonExternalId { get; } = null!;
    public string PersonFirstName { get; set; }
    public string PersonLastName { get; set; }
    public DateTime? PersonBirthday { get; set; }
    public string EventTrackingId { get; } = null!;
}