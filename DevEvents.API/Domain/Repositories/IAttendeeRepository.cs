using DevEvents.API.Domain.Entities;

namespace DevEvents.API.Domain.Repositories
{
    public interface IAttendeeRepository
    {
        Task<int> AddAsync(Attendee attendee);
    }
}
