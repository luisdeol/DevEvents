using DevEvents.API.Domain.Entities;
using DevEvents.API.Domain.Repositories;
using DevEvents.API.Infrastructure.Persistence.Models;
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
            var criteria = new SingleConferenceCriteria(id);

            var conference = await GetById(criteria);

            conference.MarkAsDeleted();

            await _db.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await _db.Conferences.AnyAsync(c => c.Id == id);
        }

        public async Task<Conference[]> GetAll(ConferencesFilterCriteria criteria)
        {
            var dbSet = _db.Conferences;
            
            var conferences = await _db.Conferences
                .AsQueryable<Conference>()
                .BuildIncludes(criteria)
                .BuildFilters(criteria)
                .BuildPagination(criteria)
                .ToArrayAsync();

            return conferences;
        }

        

        public async Task<Conference?> GetById(SingleConferenceCriteria criteria)
        {
            var conference = await _db.Conferences
                .AsQueryable<Conference>()
                .BuildIncludes(criteria)
                .SingleOrDefaultAsync(c => c.Id == criteria.Id);

            return conference;
        }

        public async Task Update(Conference conference)
        {
            _db.Conferences.Update(conference);
            await _db.SaveChangesAsync();
        }
    }

    public static class ConferenceQueryableExtensions
    {
        public static IQueryable<Conference> BuildIncludes(this IQueryable<Conference> query, ConferenceFilterCriteriaBase criteria)
        {
            if (criteria.IncludeSpeakers ?? false)
            {
                query = query.Include(q => q.Speakers);
            }

            if (criteria.IncludeRegistrations ?? false)
            {
                query = query.Include(q => q.Registrations);
            }

            return query;
        }

        public static IQueryable<Conference> BuildFilters(this IQueryable<Conference> query, ConferenceFilterCriteriaBase criteria)
        {
            if (criteria.StartDate is not null && criteria.EndDate is not null)
            {
                query = query.Where(q => q.StartDate > criteria.StartDate && q.EndDate < criteria.EndDate);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Title))
            {
                query = query.Where(q => q.Title.Contains(criteria.Title));
            }

            return query;
        }

        public static IQueryable<Conference> BuildPagination(this IQueryable<Conference> query, ConferencesFilterCriteria criteria)
        {
            query = query
                .Skip((criteria.Page - 1) * criteria.PageSize)
                .Take(criteria.PageSize);

            return query;
        }
    }
}
