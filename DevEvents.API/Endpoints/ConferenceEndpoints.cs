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

                await repository.AddAsync(conference);

                return Results.Created($"/conferences/{conference.Id}", conference);
            });

            // 🔹 Get all conferences
            app.MapGet("/conferences", async (IConferenceRepository repository) =>
                {
                    var conferences = await repository.GetAllAsync();

                    var model = conferences.Select(c => c.Adapt<ConferenceItemViewModel>());

                    return Results.Ok(model);
                }
            );

            // 🔹 Get a specific conference by ID
            app.MapGet("/conferences/{id}", async (IConferenceRepository repository, int id) =>
            {
                var conference = await repository.GetByIdAsync(id);

                var model = conference.Adapt<ConferenceItemViewModel>();

                return conference is not null ? Results.Ok(conference) : Results.NotFound();
            });

            // 🔹 Update a conference
            app.MapPut("/conferences/{id}", async (IConferenceRepository repository, int id, Conference updatedConference) =>
            {
                var existingConference = await repository.GetByIdAsync(id);

                if (existingConference is null) return Results.NotFound();

                existingConference.Update(updatedConference.Title, updatedConference.Description, updatedConference.StartDate, updatedConference.EndDate);
                
                await repository.UpdateAsync(existingConference);

                return Results.NoContent();
            });

            // 🔹 Delete a conference
            app.MapDelete("/conferences/{id}", async (IConferenceRepository repository, int id) =>
            {
                var conferenceExists = await repository.ExistsAsync(id);

                if (!conferenceExists) return Results.NotFound();

                await repository.DeleteAsync(id);

                return Results.NoContent();
            });

            // 🔹 Add an registration to a conference
            app.MapPost("/conferences/{id}/registrations", async (
                IConferenceRepository conferenceRepository, 
                IAttendeeRepository attendeeRepository, 
                IUnitOfWork unitOfWork,
                int id, RegistrationInputModel model) =>
            {
                var attendee = new Attendee(model.AttendeeName, model.AttendeeEmail);

                try
                {
                    await unitOfWork.BeginTransactionAsync();

                    await unitOfWork.Attendees.AddAsync(attendee);

                    await unitOfWork.SaveAsync();

                    var registration = new Registration(id, attendee.Id);

                    await unitOfWork.Conferences.AddRegistrationAsync(registration);

                    await unitOfWork.SaveAsync();

                    await unitOfWork.CommitTransactionAsync();

                    return Results.NoContent();
                } catch (Exception)
                {
                    await unitOfWork.RollbackTransactionAsync();

                    return Results.Problem("Error when registering participant.");
                }
            });

            // 🔹 Add a speaker to a conference
            app.MapPost("/conferences/{id}/speakers", async (IConferenceRepository repository, int id, Speaker speaker) =>
            {
                speaker.IdConference = id;

                await repository.AddSpeakerAsync(speaker);

                return Results.NoContent();
            });

            return app;
        }
    }
}
