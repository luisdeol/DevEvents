using DevEvents.API.Infrastructure.Notifications;
using Hangfire;

namespace DevEvents.API.Endpoints
{
    public static class JobsEndpoints
    {
        public static WebApplication AddJobsEndpoints(this WebApplication app)
        {
            var to = "sampleemail@mail.com";
            var subject = "Sample subject";

            app.MapGet("fire-and-forget", (
                INotificationService notificationService,
                IBackgroundJobClient client) =>
            {
                client.Enqueue(() => notificationService.SendEmail(to, subject, "Fire and Forget"));
            });

            app.MapGet("fire-and-forget-continuation", (
                INotificationService notificationService,
                IBackgroundJobClient client) =>
            {
                var id = client.Enqueue(() => notificationService.SendEmail(to, subject, "Fire and Forget"));

                client.ContinueJobWith(id, () => notificationService.SendEmail(to, subject, "Continuation"));
            });

            app.MapGet("fire-and-forget-delayed", (
                INotificationService notificationService,
                IBackgroundJobClient client) =>
            {
                client.Schedule(() => notificationService.SendEmail(to, subject, "Fire and Forget - Delayed"), TimeSpan.FromSeconds(5));
            });

            app.MapGet("recurring", (
                INotificationService notificationService,
                IRecurringJobManager manager) =>
            {
                var currentMinute = DateTime.Now.Minute + 1;

                var cronExpression = $"*/{currentMinute} * * * *";

                manager.AddOrUpdate($"recurring-job-{Guid.NewGuid()}", () => notificationService.SendEmail(to, subject, "Recurring"), cronExpression);
            });

            app.MapGet("cancel-job/{id}", (
                string id,
                IRecurringJobManager manager) =>
            {
                manager.RemoveIfExists(id);
            });

            return app;
        }
    }
}
