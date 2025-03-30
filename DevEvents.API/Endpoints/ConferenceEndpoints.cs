using DevEvents.API.Domain.Entities;
using DevEvents.API.Domain.Repositories;
using DevEvents.API.Infrastructure.Persistence;
using DevEvents.API.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace DevEvents.API.Endpoints
{
    public static class ConferenceEndpoints
    {
        public static WebApplication AddConferenceEndpoints(this WebApplication app)
        {
            // 🔹 Create a conference
            app.MapPost("/conferences", async (
                IConferenceRepository repository, 
                AddConferenceInputModel model) =>
            {
                var conference = model.Adapt<Conference>();

                await repository.Add(conference);

                return Results.Created($"/conferences/{conference.Id}", conference);
            });

            // 🔹 Get all conferences
            app.MapGet("/conferences", async (IConferenceRepository repository, IMemoryCache cache) =>
                {
                    const string cacheKey = "conferences";

                    //if (!cache.TryGetValue(cacheKey, out List<ConferenceItemViewModel>? conferences))
                    //{
                    //    var conferencesDb = await repository.GetAll();

                    //    conferences = conferencesDb.Select(c => c.Adapt<ConferenceItemViewModel>()).ToList();

                    //    var cacheEntryOptions = new MemoryCacheEntryOptions()
                    //        .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                    //        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    //    cache.Set(cacheKey, conferences, cacheEntryOptions);
                    //}

                    var conferences = await cache.GetOrCreateAsync(cacheKey, async entry =>
                    {
                        entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                        entry.AbsoluteExpiration = DateTime.UtcNow.AddHours(1);

                        var conferencesDb = await repository.GetAll();

                        var model = conferencesDb.Select(c => c.Adapt<ConferenceItemViewModel>()).ToList();

                        return model;
                    });
                    
                    return Results.Ok(conferences);
                }
            );

            // 🔹 Get a specific conference by ID
            app.MapGet("/conferences/{id}", async (IConferenceRepository repository, int id, IMemoryCache cache) =>
            {
                var cacheKey = $"conferences:{id}";

                var conference = await cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                    entry.AbsoluteExpiration = DateTime.UtcNow.AddHours(1);

                    var conferenceDb = await repository.GetById(id);

                    var model = conferenceDb.Adapt<ConferenceItemViewModel>();

                    return model;
                });

                return conference is not null ? Results.Ok(conference) : Results.NotFound();
            });

            // 🔹 Update a conference
            app.MapPut("/conferences/{id}", async (IConferenceRepository repository, int id, Conference updatedConference) =>
            {
                var existingConference = await repository.GetById(id);

                if (existingConference is null) return Results.NotFound();

                existingConference.Update(updatedConference.Title, updatedConference.Description, updatedConference.StartDate, updatedConference.EndDate);
                
                await repository.Update(existingConference);

                return Results.NoContent();
            });

            // 🔹 Delete a conference
            app.MapDelete("/conferences/{id}", async (IConferenceRepository repository, int id) =>
            {
                var conferenceExists = await repository.Exists(id);

                if (!conferenceExists) return Results.NotFound();

                await repository.Delete(id);

                return Results.NoContent();
            });

            // 🔹 Add an registration to a conference
            app.MapPost("/conferences/{id}/registrations", async (IConferenceRepository repository, int id, Attendee attendee) =>
            {
                await repository.AddRegistrationFromAttendee(id, attendee);

                return Results.NoContent();
            });

            // 🔹 Add a speaker to a conference
            app.MapPost("/conferences/{id}/speakers", async (IConferenceRepository repository, int id, Speaker speaker) =>
            {
                speaker.IdConference = id;

                await repository.AddSpeaker(speaker);

                return Results.NoContent();
            });

            return app;
        }
    }
}
