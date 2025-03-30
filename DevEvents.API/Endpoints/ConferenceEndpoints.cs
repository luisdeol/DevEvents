using DevEvents.API.Domain.Entities;
using DevEvents.API.Domain.Repositories;
using DevEvents.API.Infrastructure.Persistence;
using DevEvents.API.Models;
using Mapster;
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

                // return Results.Created($"/conferences/{conference.Id}", conference);

                return conference.ToCreatedResult($"/conferences/{conference.Id}");
            });

            // 🔹 Get all conferences
            app.MapGet("/conferences", async (IConferenceRepository repository) =>
                {
                    var conferences = await repository.GetAll();

                    var model = conferences.Select(c => c.Adapt<ConferenceItemViewModel>()).ToList();

                    // var result = ResultData<List<ConferenceItemViewModel>>.Success(model);

                    // return Results.Ok(result);

                    return model.ToOkResult();
                }
            );

            // 🔹 Get a specific conference by ID
            app.MapGet("/conferences/{id}", async (IConferenceRepository repository, int id) =>
            {
                var conference = await repository.GetById(id);

                var model = conference.Adapt<ConferenceItemViewModel>();

                //ResultData<ConferenceItemViewModel> result;

                //if (model is null)
                //{
                //    var message = "Not found";

                //    result = ResultData<ConferenceItemViewModel>.Error(message);

                //    return Results.NotFound(result);
                //}
                //else
                //{
                //    result = ResultData<ConferenceItemViewModel>.Success(model);
                //}

                //return Results.Ok(result);

                return model.ToSingleResult();
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
