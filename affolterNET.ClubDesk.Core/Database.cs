using System.Data.Common;
using System.Data.SQLite;
using affolterNET.ClubDesk.Core.Models;
using Dapper;

namespace affolterNET.ClubDesk.Core;

public class Database
{
    private readonly string _dbfile;

    public Database(string filename)
    {
        _dbfile = filename;
        if (!File.Exists(_dbfile))
        {
            SQLiteConnection.CreateFile(_dbfile);
        }

        EnsureTables().GetAwaiter().GetResult();
    }
    
    public async Task DeleteEventsAndInvitationsOfYears(List<int> years)
    {
        var cn = GetConn();
        var yearString = $"'{string.Join("','", years)}'";
        await cn.ExecuteAsync(
            $"delete from invitation where EventId in (select EventId from Event where strftime('%Y', Start) in ({yearString}))");
        await cn.ExecuteAsync($"delete from Event where strftime('%Y', Start) in ({yearString})");
    }

    public async Task<List<Person>> UpdatePersons(List<Person> pers)
    {
        var cn = GetConn();
        List<Person> dbPersons = new();
        foreach (var p in pers)
        {
            var exists = await cn.QueryFirstOrDefaultAsync(@"select PersonId from Person where PersonId=@PersonId", p);
            if (exists == null)
            {
                await InsertPerson(cn, p);
            }
            else
            {
                await UpdatePerson(cn, p);
            }
        }
        return (await cn.QueryAsync<Person>("select * from Person")).ToList();;
    }

    public async Task<List<Event>> UpdateEvents(Dictionary<int, ParsedLists> years)
    {
        var cn = GetConn();
        foreach (var year in years.Keys)
        {
            foreach (var e in years[year].Events)
            {
                await cn.ExecuteAsync("insert into Event (TrackingId, Start, End, Subject, Invited, Yes, No, None, MayBe) values (@TrackingId, @Start, @End, @Subject, @Invited, @Yes, @No, @None, @MayBe)",
                    e);
            }
        }
        var dbEvents = (await cn.QueryAsync<Event>("select * from Event")).ToList();
        return dbEvents;
    }
    
    public async Task UpdateInvitations(Dictionary<int,ParsedLists> years, List<Person> persons, List<Event> events)
    {
        var cn = GetConn();
        foreach (var year in years.Keys)
        {
            foreach (var i in years[year].Invitations)
            {
                Event dbEvent;
                try
                {
                    dbEvent = events.Single(e => e.TrackingId == i.EventTrackingId);
                }
                catch
                {
                    Console.WriteLine("problem with event tracking id: {0}", i.EventTrackingId);
                    throw;
                }

                Person dbPerson;
                try
                {
                    dbPerson = persons.Single(p => p.ExternalId == i.PersonExternalId);
                    if (dbPerson == null)
                    {
                        dbPerson = persons.Where(p => p.Firstname == i.)
                    }
                }
                catch
                {
                    Console.WriteLine("problem with person: {0}", i.PersonExternalId);
                    throw;
                }


                i.EventId = dbEvent.EventId;
                i.PersonId = dbPerson.PersonId;
                await cn.ExecuteAsync("insert into Invitation (EventId, PersonId, StatusId) values (@EventId, @PersonId, @StatusId)",
                    i);
            }
        }
    }

    private DbConnection GetConn()
    {
        return new SQLiteConnection($"Data Source={_dbfile};Version=3;Compress=True;");
    }

    private async Task EnsureTables()
    {
        await EnsureTable("Event", $@"
            create table Event (
                EventId integer primary key,
                TrackingId nvarchar(50) not null unique,
                Start datetime,
                End datetime,
                Subject nvarchar(1000),
                Invited int null,
                Yes int null,
                No int null,
                None int null,
                MayBe int null
            );
        ");

        await EnsureTable("Person", $@"
            create table Person (
                PersonId integer primary key,
                ExternalId nvarchar(100) not null,
                Firstname nvarchar(100),
                Lastname nvarchar(100),
                Birthday datetime,
                Vintage nvarchar(100),
                Company nvarchar(100),
                Address nvarchar(100),
                AddressSuffix nvarchar(100),
                Zip nvarchar(10),
                City nvarchar(100),
                Country nvarchar(100),
                Nationality nvarchar(100),
                PhonePrivate nvarchar(100),
                PhoneBusiness nvarchar(100),
                PhoneMobile nvarchar(100),
                Fax nvarchar(100),
                Salutation nvarchar(100),
                LetterSalutation nvarchar(100),
                Email1 nvarchar(100),
                Email2 nvarchar(100),
                Groups nvarchar(200),
                Roles nvarchar(200),
                MemberStatus nvarchar(100),
                AccessionDate datetime,
                ExitDate datetime,
                MaritalStatus nvarchar(100),
                Sex nvarchar(100),
                Comments text,
                BusinessWebsite nvarchar(100),
                BillByEmail bit,
                RemindOverdue bit,
                Iban nvarchar(50),
                Bic nvarchar(50),
                AccountHolder nvarchar(100),
                Title nvarchar(100),
                PlayerNumber nvarchar(100),
                Association nvarchar(100),
                MemberYears int,
                Age int,
                ChangedAt datetime,
                ChangedFrom nvarchar(100)
            );
        ");

        await EnsureTable("Status", @"
            create table Status (
                StatusId integer primary key,
                Description nvarchar(20)
            );
            insert into Status (StatusId, Description) values (1, 'Ja');
            insert into Status (StatusId, Description) values (2, 'Nein');
            insert into Status (StatusId, Description) values (3, 'None');
        ");

        await EnsureTable("Invitation", $@"
            create table Invitation (
                EventId integer NOT NULL,
                PersonId integer NOT NULL,
                StatusId integer NOT NULL,
                PRIMARY KEY (EventId, PersonId),
                FOREIGN KEY(EventId) REFERENCES Event(EventId),
                FOREIGN KEY(PersonId) REFERENCES Person(PersonId),
                FOREIGN KEY(StatusId) REFERENCES Status(StatusId)
            );
        ");
    }

    private async Task EnsureTable(string tableName, string create)
    {
        var cn = GetConn();
        var table = await cn.QueryAsync<string>(
            $"SELECT name FROM sqlite_master WHERE type='table' AND name = '{tableName}';");
        var name = table.FirstOrDefault();
        if (!string.IsNullOrEmpty(name) && name == tableName)
            return;

        await cn.ExecuteAsync(create);
    }

    private async Task UpdatePerson(DbConnection cn, Person p)
    {
        await cn.ExecuteAsync(@"
            update Person set
                PersonId=@PersonId,
                Firstname=@Firstname,
                Lastname=@Lastname,
                ExternalId=@ExternalId,
                Birthday=@Birthday,
                Vintage=@Vintage,
                Company=@Company,
                Address=@Address,
                AddressSuffix=@AddressSuffix,
                Zip=@Zip,
                City=@City,
                Country=@Country,
                Nationality=@Nationality,
                PhonePrivate=@PhonePrivate,
                PhoneBusiness=@PhoneBusiness,
                PhoneMobile=@PhoneMobile,
                Fax=@Fax,
                Salutation=@Salutation,
                LetterSalutation=@LetterSalutation,
                Email1=@Email1,
                Email2=@Email2,
                Groups=@Groups,
                Roles=@Roles,
                MemberStatus=@MemberStatus,
                AccessionDate=@AccessionDate,
                ExitDate=@ExitDate,
                MaritalStatus=@MaritalStatus,
                Sex=@Sex,
                Comments=@Comments,
                BusinessWebsite=@BusinessWebsite,
                BillByEmail=@BillByEmail,
                RemindOverdue=@RemindOverdue,
                Iban=@Iban,
                Bic=@Bic,
                AccountHolder=@AccountHolder,
                Title=@Title,
                PlayerNumber=@PlayerNumber,
                Association=@Association,
                MemberYears=@MemberYears,
                Age=@Age,
                ChangedAt=@ChangedAt,
                ChangedFrom=@ChangedFrom
            where PersonId=@PersonId
        ", p);
    }

    private async Task InsertPerson(DbConnection cn, Person p)
    {
        await cn.ExecuteAsync(@"
            insert into Person (
                PersonId,
                Firstname,
                Lastname,
                ExternalId,
                Birthday,
                Vintage,
                Company,
                Address,
                AddressSuffix,
                Zip,
                City,
                Country,
                Nationality,
                PhonePrivate,
                PhoneBusiness,
                PhoneMobile,
                Fax,
                Salutation,
                LetterSalutation,
                Email1,
                Email2,
                Groups,
                Roles,
                MemberStatus,
                AccessionDate,
                ExitDate,
                MaritalStatus,
                Sex,
                Comments,
                BusinessWebsite,
                BillByEmail,
                RemindOverdue,
                Iban,
                Bic,
                AccountHolder,
                Title,
                PlayerNumber,
                Association,
                MemberYears,
                Age,
                ChangedAt,
                ChangedFrom
            ) values (
                @PersonId,
                @Firstname,
                @Lastname,
                @ExternalId,
                @Birthday,
                @Vintage,
                @Company,
                @Address,
                @AddressSuffix,
                @Zip,
                @City,
                @Country,
                @Nationality,
                @PhonePrivate,
                @PhoneBusiness,
                @PhoneMobile,
                @Fax,
                @Salutation,
                @LetterSalutation,
                @Email1,
                @Email2,
                @Groups,
                @Roles,
                @MemberStatus,
                @AccessionDate,
                @ExitDate,
                @MaritalStatus,
                @Sex,
                @Comments,
                @BusinessWebsite,
                @BillByEmail,
                @RemindOverdue,
                @Iban,
                @Bic,
                @AccountHolder,
                @Title,
                @PlayerNumber,
                @Association,
                @MemberYears,
                @Age,
                @ChangedAt,
                @ChangedFrom
            )
        ", p);
    }
}