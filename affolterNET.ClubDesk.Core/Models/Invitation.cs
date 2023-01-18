namespace affolterNET.ClubDesk.Core.Models;

public class Invitation
{
    public Invitation()
    {
    }

    public Invitation(Status status, string eventTrackingId, string personFirstname, string personLastname, DateTime? personBirthday, string personExternalId)
    {
        PersonExternalId = personExternalId;
        EventTrackingId = eventTrackingId;
        PersonFirstname = personFirstname;
        PersonLastname = personLastname;
        PersonBirthday = personBirthday;
        StatusId = (int)status;
    }

    public int EventId { get; set; }
    public int PersonId { get; set; }
    public int StatusId { get; set; }

    public Status Status => (Status)StatusId;

    public DateTime? PersonBirthday { get; set; }
    public string EventTrackingId { get; } = null!;
    public string PersonFirstname { get; } = null!;
    public string PersonLastname { get; } = null!;
    public string PersonExternalId { get; } = null!;
}