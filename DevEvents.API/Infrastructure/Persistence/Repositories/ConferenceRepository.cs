using DevEvents.API.Domain.Entities;
using DevEvents.API.Domain.Repositories;
using DevEvents.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DevEvents.API.Infrastructure.Persistence.Repositories
{
    public class ConferenceRepository : IConferenceRepository
    {
        private readonly AppDbContext _db;
        public ConferenceRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> Add(Conference conference)
        {
            _db.Conferences.Add(conference);
            await _db.SaveChangesAsync();

            return conference.Id;
        }

        public async Task AddRegistrationFromAttendee(int idConference, Attendee attendee)
        {
            await _db.Attendees.AddAsync(attendee);
            await _db.SaveChangesAsync();

            var registration = new Registration(idConference, attendee.Id);

            await _db.Registrations.AddAsync(registration);
            await _db.SaveChangesAsync();
        }

        public async Task AddSpeaker(Speaker speaker)
        {
            await _db.Speakers.AddAsync(speaker);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var conference = await GetById(id);

            conference.MarkAsDeleted();

            await _db.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await _db.Conferences.AnyAsync(c => c.Id == id);
        }

        public async Task<Conference[]> GetAll()
        {
            var conferences = await _db.Conferences
                            .Include(c => c.Speakers)
                            .Include(c => c.Registrations)
                        .ToArrayAsync();

            return conferences;
        }

        public async Task<Conference?> GetById(int id)
        {
            var conference = await _db.Conferences
                    .Include(c => c.Speakers)
                    .Include(c => c.Registrations)
                        .ThenInclude(r => r.Attendee)
                    .SingleOrDefaultAsync(c => c.Id == id);

            return conference;
        }

        public async Task Update(Conference conference)
        {
            _db.Conferences.Update(conference);
            await _db.SaveChangesAsync();
        }
    }
}
