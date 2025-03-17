namespace DevEvents.API.Infrastructure.Notifications
{
    public interface INotificationService
    {
        Task<bool> SendEmail(string to, string subject, string message);
    }
}
