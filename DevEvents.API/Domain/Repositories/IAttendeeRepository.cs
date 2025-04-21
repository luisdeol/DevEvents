using DevEvents.API.Domain.Entities;

namespace DevEvents.API.Domain.Repositories
{
    public interface IAttendeeRepository
    {
        Task<int> Add(Attendee attendee);
    }
}
