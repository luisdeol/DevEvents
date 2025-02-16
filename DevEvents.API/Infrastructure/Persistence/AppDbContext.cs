using DevEvents.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevEvents.API.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Conference> Conferences { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Conference>(e =>
            {
                e.HasKey(c => c.Id);

                e.HasMany(c => c.Speakers)
                    .WithOne()
                    .HasForeignKey(s => s.IdConference)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(c => c.Registrations)
                    .WithOne(r => r.Conference)
                    .HasForeignKey(s => s.IdConference)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Speaker>(e =>
            {
                e.HasKey(c => c.Id);
            });

            modelBuilder.Entity<Attendee>(e =>
            {
                e.HasKey(c => c.Id);

                e.HasOne(a => a.Registration)
                    .WithOne(r => r.Attendee)
                    .HasForeignKey<Registration>(r => r.IdAttendee)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Registration>(e =>
            {
                e.HasKey(c => c.Id);
            });
        }
    }
}
