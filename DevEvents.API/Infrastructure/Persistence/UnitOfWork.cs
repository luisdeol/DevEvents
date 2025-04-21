using DevEvents.API.Domain.Repositories;
using DevEvents.API.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevEvents.API.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly AppDbContext _context;
        IDbContextTransaction? _transaction;

        IConferenceRepository? _conferenceRepository;
        IAttendeeRepository? _attendeeRepository;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IConferenceRepository Conferences => 
            _conferenceRepository ??= new ConferenceRepository(_context);

        public IAttendeeRepository Attendees => 
            _attendeeRepository ??= new AttendeeRepository(_context);

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
                _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _context.SaveChangesAsync();

                await _transaction.CommitAsync();

                await _transaction.DisposeAsync();

                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();

                await _transaction.DisposeAsync();

                _transaction = null;
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dipose()
        {
            _transaction?.Dispose();

            _context.Dispose();
        }
    }
}
