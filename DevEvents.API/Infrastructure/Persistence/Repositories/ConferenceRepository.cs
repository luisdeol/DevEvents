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

        public async Task<int> AddAsync(Conference conference)
        {
            _db.Conferences.Add(conference);
            await _db.SaveChangesAsync();

            return conference.Id;
        }

        public async Task AddRegistrationAsync(Registration registration)
        {
            await _db.Registrations.AddAsync(registration);
            await _db.SaveChangesAsync();
        }

        public async Task AddSpeakerAsync(Speaker speaker)
        {
            await _db.Speakers.AddAsync(speaker);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var conference = await GetByIdAsync(id);

            conference.MarkAsDeleted();

            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Conferences.AnyAsync(c => c.Id == id);
        }

        public async Task<Conference[]> GetAllAsync()
        {
            var conferences = await _db.Conferences
                            .Include(c => c.Speakers)
                            .Include(c => c.Registrations)
                        .ToArrayAsync();

            return conferences;
        }

        public async Task<Conference?> GetByIdAsync(int id)
        {
            var conference = await _db.Conferences
                    .Include(c => c.Speakers)
                    .Include(c => c.Registrations)
                    .SingleOrDefaultAsync(c => c.Id == id);

            return conference;
        }

        public async Task UpdateAsync(Conference conference)
        {
            _db.Conferences.Update(conference);
            await _db.SaveChangesAsync();
        }
    }
}
