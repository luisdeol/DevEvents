namespace DevEvents.API.Infrastructure.Notifications
{
    public class EmailService : INotificationService
    {
        public Task<bool> SendEmail(string to, string subject, string message)
        {
            Console.WriteLine($"Email sent to {to}, subject: {subject}, message: {message}");

            return Task.FromResult(true);
        }
    }
}
