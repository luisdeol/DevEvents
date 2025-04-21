using DevEvents.API.Domain.Repositories;

namespace DevEvents.API.Infrastructure.Persistence
{
    public interface IUnitOfWork
    {
        IConferenceRepository Conferences { get; }
        IAttendeeRepository Attendees { get; }

        Task SaveAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
