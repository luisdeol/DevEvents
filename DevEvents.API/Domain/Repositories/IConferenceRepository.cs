using DevEvents.API.Domain.Entities;

namespace DevEvents.API.Domain.Repositories
{
    public interface IConferenceRepository
    {
        Task<int> AddAsync(Conference conference);
        Task<Conference[]> GetAllAsync();
        Task<Conference?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task UpdateAsync(Conference conference);
        Task DeleteAsync(int id);
        Task AddRegistrationAsync(Registration registration);
        Task AddSpeakerAsync(Speaker speaker);
    }
}
