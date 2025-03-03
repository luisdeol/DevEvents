using Dapper;
using DevEvents.API.Domain.Entities;
using DevEvents.API.Domain.Repositories;
using DevEvents.API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DevEvents.API.Infrastructure.Persistence.Repositories
{
    public class ConferenceRepository : IConferenceRepository
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        public ConferenceRepository(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        private IDbConnection Connection => new SqlConnection(_configuration.GetConnectionString("AppDb"));

        public async Task<int> Add(Conference conference)
        {
            var sql = @"INSERT INTO Conferences (Title, Description, StartDate, EndDate, CreatedAt, IsDeleted)
                VALUES (@Title, @Description, @StartDate, @EndDate, @CreatedAt, @IsDeleted);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = Connection;

            var id = await connection.ExecuteScalarAsync<int>(sql, conference);

            return id;
        }

        public async Task AddRegistrationFromAttendee(int idConference, Attendee attendee)
        {
            var sqlAttendee = @"INSERT INTO Attendees (Name, Email, CreatedAt, IsDeleted)
                VALUES (@Name, @Email, @CreatedAt, @IsDeleted);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = Connection;

            var attendeeId = await connection.ExecuteScalarAsync<int>(sqlAttendee, attendee);

            var sqlRegistration = @"INSERT INTO Registrations (IdConference, IdAttendee, CreatedAt, IsDeleted)
                VALUES (@IdConference, @IdAttendee, @CreatedAt, @IsDeleted)";

            await connection.ExecuteAsync(sqlRegistration, new { 
                IdConference = idConference,
                IdAttendee = attendeeId,
                CreatedAt = attendee.CreatedAt,
                IsDeleted = false
            });
        }

        public async Task AddSpeaker(Speaker speaker)
        {
            var sql = @"INSERT INTO Speakers (Name, Bio, Website, IdConference, CreatedAt, IsDeleted)
                VALUES (@Name, @Bio, @Website, @IdConference, @CreatedAt, @IsDeleted);";

            using var connection = Connection;

            await connection.ExecuteAsync(sql, speaker);
        }

        public async Task Delete(int id)
        {
            var sql = "UPDATE Conferences SET IsDeleted = 1 WHERE Id = @Id";

            using var connection = Connection;

            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<bool> Exists(int id)
        {
            var sql = "SELECT COUNT(1) FROM Conferences WHERE Id = @Id AND IsDeleted = 0";

            using var connection = Connection;

            var exists = await connection.ExecuteScalarAsync<bool>(sql, new { Id = id });

            return exists;
        }

        public async Task<Conference[]> GetAll()
        {
            var sql = @"SELECT * FROM Conferences WHERE IsDeleted = 0;
                        SELECT * FROM Speakers WHERE IdConference IN (SELECT Id From Conferences WHERE IsDeleted = 0);
                        SELECT * FROM Registrations WHERE IdConference IN (SELECT Id From Conferences WHERE IsDeleted = 0);";

            using var connection = Connection;

            using var multipleQueries = await connection.QueryMultipleAsync(sql);

            var conferences = (await multipleQueries.ReadAsync<Conference>()).ToList();
            var speakers = (await multipleQueries.ReadAsync<Speaker>()).ToList();
            var registrations = (await multipleQueries.ReadAsync<Registration>()).ToList();

            foreach (var conference in conferences)
            {
                conference.Speakers = speakers.Where(s => s.IdConference == conference.Id).ToList();
                conference.Registrations = registrations.Where(r => r.IdConference == conference.Id).ToList();
            }

            return conferences.ToArray();
        }

        public async Task<Conference?> GetById(int id)
        {
            var sql = @"SELECT c.Id, c.Title, c.Description, c.StartDate, c.EndDate, c.CreatedAt, c.IsDeleted,
                                s.Id, s.Name, s.Bio, s.Website, s.IdConference,
                                r.Id, r.idConference, r.IdAttendee
                        FROM Conferences c 
                        LEFT JOIN Speakers s ON c.Id = s.IdConference 
                        LEFT JOIN Registrations r ON c.Id = r.IdConference 
                        WHERE c.Id = @Id AND c.IsDeleted = 0;";

            using var connection = Connection;

            var conferenceDictionary = new Dictionary<int, Conference>();

            var conferences = await connection.QueryAsync<Conference, Speaker, Registration, Conference>(
                sql, (conference, speaker, registration) =>
                {
                    if (!conferenceDictionary.TryGetValue(conference.Id, out var conf))
                    {
                        conf = conference;
                        conf.Speakers = [];
                        conf.Registrations = [];
                        conferenceDictionary[conference.Id] = conf;
                    }

                    if (speaker != null && !conf.Speakers.Any(s => s.Id == speaker.Id))
                    {
                        conf.Speakers.Add(speaker);
                    }

                    if (registration != null && !conf.Registrations.Any(r => r.Id == registration.Id))
                    {
                        conf.Registrations.Add(registration);
                    }

                    return conf;
                },
                new { Id = id },
                splitOn: "Id,Id");

            return conferences.FirstOrDefault();
        }

        public async Task Update(Conference conference)
        {
            var sql = @"UPDATE Conferences
                SET Title = @Title, Description = @Description, StartDate = @StartDate, EndDate = @EndDate
                WHERE Id = @Id;";

            using var connection = Connection;

            await connection.ExecuteAsync(sql, conference);
        }
    }
}
