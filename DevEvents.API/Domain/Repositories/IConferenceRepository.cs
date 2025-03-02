using DevEvents.API.Domain.Entities;

namespace DevEvents.API.Domain.Repositories
{
    public interface IConferenceRepository
    {
        Task<int> Add(Conference conference);
        Task<Conference[]> GetAll();
        Task<Conference?> GetById(int id);
        Task<bool> Exists(int id);
        Task Update(Conference conference);
        Task Delete(int id);
        Task AddRegistrationFromAttendee(int idConference, Attendee attendee);
        Task AddSpeaker(Speaker speaker);
    }
}
