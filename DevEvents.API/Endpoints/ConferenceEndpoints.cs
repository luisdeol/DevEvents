using DevEvents.API.Domain.Entities;
using DevEvents.API.Domain.Repositories;
using DevEvents.API.Infrastructure.Persistence;
using DevEvents.API.Infrastructure.Persistence.Models;
using DevEvents.API.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            app.MapGet("/conferences", async (IConferenceRepository repository, [AsParameters] ConferencesFilterCriteriaModel criteria) =>
                {
                    var dbCriteria = new ConferencesFilterCriteria
                    {
                        StartDate = criteria.StartDate,
                        EndDate = criteria.EndDate,
                        Title = criteria.Title,
                        IncludeSpeakers = true,
                        IncludeRegistrations = true,
                        Page = criteria.Page ?? 1,
                        PageSize = criteria.PageSize ?? 2
                    };

                    var conferences = await repository.GetAll(dbCriteria);

                    var model = conferences.Select(c => c.Adapt<ConferenceItemViewModel>());

                    return Results.Ok(model);
                }
            );

            // 🔹 Get a specific conference by ID
            app.MapGet("/conferences/{id}", async (IConferenceRepository repository, int id) =>
            {
                var criteria = new SingleConferenceCriteria(id, true, true);

                var conference = await repository.GetById(criteria);

                var model = conference.Adapt<ConferenceItemViewModel>();

                return conference is not null ? Results.Ok(conference) : Results.NotFound();
            });

            // 🔹 Update a conference
            app.MapPut("/conferences/{id}", async (IConferenceRepository repository, int id, Conference updatedConference) =>
            {
                var criteria = new SingleConferenceCriteria(id);

                var existingConference = await repository.GetById(criteria);

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
