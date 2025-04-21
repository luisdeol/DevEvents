using DevEvents.API.Domain.Entities;
using DevEvents.API.Domain.Repositories;

namespace DevEvents.API.Infrastructure.Persistence.Repositories
{
    public class AttendeeRepository : IAttendeeRepository
    {
        private readonly AppDbContext _db;
        public AttendeeRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> AddAsync(Attendee attendee)
        {
            await _db.Attendees.AddAsync(attendee);
            await _db.SaveChangesAsync();

            return attendee.Id;
        }
    }
}
