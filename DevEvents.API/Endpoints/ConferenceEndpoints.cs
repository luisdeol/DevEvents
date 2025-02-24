using DevEvents.API.Domain.Entities;
using DevEvents.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevEvents.API.Endpoints
{
    public static class ConferenceEndpoints
    {
        public static WebApplication AddConferenceEndpoints(this WebApplication app)
        {
            app.MapPost("/conferences", async (AppDbContext db, Conference conference) =>
            {
                db.Conferences.Add(conference);
                await db.SaveChangesAsync();

                return Results.Created($"/conferences/{conference.Id}", conference);
            });

            // 🔹 Get all conferences
            app.MapGet("/conferences", async (AppDbContext db) =>
                await db.Conferences
                        .Include(c => c.Speakers)
                        .Include(c => c.Registrations)
                    .ToListAsync()
            );

            // 🔹 Get a specific conference by ID
            app.MapGet("/conferences/{id}", async (AppDbContext db, int id) =>
            {
                var conference = await db.Conferences
                    .Include(c => c.Speakers)
                    .Include(c => c.Registrations)
                    .FirstOrDefaultAsync(c => c.Id == id);

                return conference is not null ? Results.Ok(conference) : Results.NotFound();
            });

            // 🔹 Update a conference
            app.MapPut("/conferences/{id}", async (AppDbContext db, int id, Conference updatedConference) =>
            {
                var existingConference = await db.Conferences.FindAsync(id);
                if (existingConference is null) return Results.NotFound();

                existingConference.Update(updatedConference.Title, updatedConference.Description, updatedConference.StartDate, updatedConference.EndDate);
                
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // 🔹 Delete a conference
            app.MapDelete("/conferences/{id}", async (AppDbContext db, int id) =>
            {
                var conference = await db.Conferences.FindAsync(id);
                if (conference is null) return Results.NotFound();

                db.Conferences.Remove(conference);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            // 🔹 Add an registration to a conference
            app.MapPost("/conferences/{id}/registrations", async (AppDbContext db, int id, Attendee attendee) =>
            {
                var conference = await db.Conferences.FindAsync(id);

                if (conference is null) return Results.NotFound();

                await db.Attendees.AddAsync(attendee);
                await db.SaveChangesAsync();

                var registration = new Registration(id, attendee.Id);

                await db.Registrations.AddAsync(registration);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            // 🔹 Add a speaker to a conference
            app.MapPost("/conferences/{id}/speakers", async (AppDbContext db, int id, Speaker speaker) =>
            {
                var conference = await db.Conferences.FindAsync(id);

                if (conference is null) return Results.NotFound();

                speaker.IdConference = id;

                await db.Speakers.AddAsync(speaker);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            return app;
        }
    }
}
