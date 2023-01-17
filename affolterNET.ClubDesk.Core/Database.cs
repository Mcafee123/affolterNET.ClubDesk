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

    public async Task<List<Person>> UpdatePersons(Dictionary<int, ParsedLists> years)
    {
        var cn = GetConn();
        List<Person> dbPersons = new();
        foreach (var i in years.Keys)
        {
            dbPersons = (await cn.QueryAsync<Person>("select * from Person")).ToList();
            foreach (var p in years[i].Persons)
            {
                var found = dbPersons.SingleOrDefault(dbp =>
                    dbp.ExternalId == p.ExternalId);
                if (found == null)
                {
                    await cn.ExecuteAsync("insert into Person (PersonId, Firstname, Lastname, ExternalId, Birthday) values (null, @Firstname, @Lastname, @ExternalId, @Birthday)", p);
                    dbPersons = (await cn.QueryAsync<Person>("select * from Person")).ToList();
                }   
            }
        }

        return dbPersons;
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
                ExternalId nvarchar(100) not null unique,
                Firstname nvarchar(100),
                Lastname nvarchar(100),
                Birthday datetime
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
}