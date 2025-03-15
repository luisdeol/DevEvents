using DevEvents.API.Domain.Entities;
using DevEvents.API.Infrastructure.Persistence.Models;

namespace DevEvents.API.Domain.Repositories
{
    public interface IConferenceRepository
    {
        Task<int> Add(Conference conference);
        Task<Conference[]> GetAll(ConferencesFilterCriteria criteria);
        Task<Conference?> GetById(SingleConferenceCriteria criteria);
        Task<bool> Exists(int id);
        Task Update(Conference conference);
        Task Delete(int id);
        Task AddRegistrationFromAttendee(int idConference, Attendee attendee);
        Task AddSpeaker(Speaker speaker);
    }
}
