using affolterNET.ClubDesk.Core.Converters;
using CsvHelper.Configuration.Attributes;

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

    [Name("[Id]")] public int PersonId { get; set; }

    [Name("Benutzer-Id")] public string? ExternalId { get; set; }

    [Name("Geburtsdatum")][TypeConverter(typeof(CdDateOnlyConverter))] public DateTime? Birthday { get; set; }
    
    [Name("Jahrgang")] public int? Vintage { get; set; }

    [Name("Vorname")] public string Firstname { get; set; } = null!;

    [Name("Nachname")] public string Lastname { get; set; } = null!;

    [Name("Firma")] public string? Company { get; set; }

    [Name("Adresse")] public string? Address { get; set; }
    
    [Name("Adress-Zusatz")] public string? AddressSuffix { get; set; }

    [Name("PLZ")] public string? Zip { get; set; }
    
    [Name("Ort")] public string? City { get; set; }
    
    [Name("Land")] public string? Country { get; set; }
    
    [Name("Nationalität")] public string? Nationality { get; set; }
    
    [Name("Telefon Privat")] public string? PhonePrivate { get; set; }
    
    [Name("Telefon Geschäft")] public string? PhoneBusiness { get; set; }
    
    [Name("Telefon Mobil")] public string? PhoneMobile { get; set; }
    
    [Name("Fax")] public string? Fax { get; set; }
    
    [Name("Anrede")] public string? Salutation { get; set; }
    
    [Name("Briefanrede")] public string? LetterSalutation { get; set; }
    
    [Name("E-Mail")] public string? Email1 { get; set; }
    
    [Name("E-Mail Alternativ")] public string? Email2 { get; set; }
    
    [Name("[Gruppen]")] public string? Groups { get; set; }
    
    [Name("[Rolle]")] public string? Roles { get; set; }
    
    [Name("Status")] public string? MemberStatus { get; set; }
    
    [Name("Eintritt")][TypeConverter(typeof(CdDateOnlyConverter))] public DateTime? AccessionDate { get; set; }
    
    [Name("Austritt")][TypeConverter(typeof(CdDateOnlyConverter))] public DateTime? ExitDate { get; set; }
    
    [Name("Zivilstand")] public string? MaritalStatus { get; set; }
    
    [Name("Geschlecht")] public string? Sex { get; set; }
    
    [Name("Bemerkungen")] public string? Comments { get; set; }
    
    [Name("Firmen-Webseite")] public string? BusinessWebsite { get; set; }
 
    [Name("Rechnungsversand")][TypeConverter(typeof(BillByEmailConverter))] public bool BillByEmail { get; set; }
    
    [Name("Nie mahnen")][TypeConverter(typeof(RemindOverdueConverter))] public bool RemindOverdue { get; set; }
    
    [Name("IBAN")] public string? Iban { get; set; }
    
    [Name("BIC")] public string? Bic { get; set; }
    
    [Name("Kontoinhaber")] public string? AccountHolder { get; set; }
    
    [Name("Titel")] public string? Title { get; set; }
    
    [Name("Spieler Nummer")] public string? PlayerNumber { get; set; }

    [Name("Vereinszugehörigkeit")] public string? Association { get; set; }
    
    [Name("Mitgliedsjahre")] public int? MemberYears { get; set; }
    
    [Name("Alter")] public int? Age { get; set; }
    
    [Name("[Zuletzt geändert am]")][TypeConverter(typeof(CdDateTimeConverter))] public DateTime? ChangedAt { get; set; }
    
    [Name("[Zuletzt geändert von]")] public string? ChangedFrom { get; set; }
}