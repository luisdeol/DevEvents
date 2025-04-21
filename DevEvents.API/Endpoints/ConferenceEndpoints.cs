using DevEvents.API.Domain.Entities;
using DevEvents.API.Domain.Repositories;
using DevEvents.API.Infrastructure.Persistence;
using DevEvents.API.Infrastructure.Storage;
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
                IAttendeeRepository attendeeRepository, int id, RegistrationInputModel model) =>
            {
                var attendee = new Attendee(model.AttendeeName, model.AttendeeEmail);

                var idAttendee = await attendeeRepository.AddAsync(attendee);

                var registration = new Registration(id, idAttendee);

                await conferenceRepository.AddRegistrationAsync(registration);

                return Results.NoContent();
            });

            // 🔹 Add a speaker to a conference
            app.MapPost("/conferences/{id}/speakers", async (IConferenceRepository repository, int id, Speaker speaker) =>
            {
                speaker.IdConference = id;

                await repository.AddSpeakerAsync(speaker);

                return Results.NoContent();
            });

            app.MapPost("/conferences/{id}/photos", async (
                [FromServices] IStorageService storageService,
                string id,
                IFormFile file) =>
            {
                if (file is null || file.Length == 0)
                {
                    return Results.BadRequest("File not found.");
                }

                using var stream = file.OpenReadStream();

                var success = await storageService.UploadPhoto(id, file.FileName, stream);
                
                if (!success)
                {
                    return Results.InternalServerError();
                }

                var blobName = $"{id}/{file.FileName}";

                return Results.Ok(new { Path = blobName, Message = "Photo was uploaded successfully." });
            }).DisableAntiforgery();

            app.MapGet("/conferences/{id}/photos/{fileName}", async (
                IStorageService storageService,
                string id,
                string fileName) =>
            {
                var stream = await storageService.DownloadPhoto(id, fileName);

                if (stream == null)
                {
                    return Results.NotFound("File not found.");
                }

                var contentType = GetContentType(fileName);

                return Results.File(stream, contentType, fileName);
            });

            return app;
        }

        static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}
