using DevEvents.API.Domain.Entities;
using DevEvents.API.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DevEvents.API.Infrastructure.Persistence.Repositories
{
    public class CachedConferenceRepository : IConferenceRepository
    {
        readonly IConferenceRepository _repository;
        readonly IDistributedCache _cache;
        public CachedConferenceRepository(
            IConferenceRepository repository,
            IDistributedCache cache
            )
        {
            _repository = repository;
            _cache = cache;
        }
        public Task<int> Add(Conference conference)
        {
            throw new NotImplementedException();
        }

        public Task AddRegistrationFromAttendee(int idConference, Attendee attendee)
        {
            throw new NotImplementedException();
        }

        public Task AddSpeaker(Speaker speaker)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Conference[]> GetAll()
        {
            var cacheKey = "conferences";

            var cachedConferences = await _cache.GetStringAsync(cacheKey);

            Conference[] conferences;

            if (cachedConferences is not null)
            {
                conferences = JsonSerializer.Deserialize<Conference[]>(cachedConferences);
            }
            else
            {
                conferences = await _repository.GetAll();

                var options = new DistributedCacheEntryOptions();
                options.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(conferences), options);
            }

            return conferences;
        }

        public Task<Conference?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task Update(Conference conference)
        {
            throw new NotImplementedException();
        }
    }
}
