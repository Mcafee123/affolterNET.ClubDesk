namespace affolterNET.ClubDesk.Core.Models;

public class Person
{
    public Person()
    {
    }

    public Person(string first, string last)
    {
        Firstname = first;
        Lastname = last;
    }

    public int PersonId { get; set; }
    public string? ExternalId { get; set; }
    public DateTime? Birthday { get; set; }
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
}