using VoiceAgentRag.Application.Abstractions.Persistence;

namespace VoiceAgentRag.Infrastructure.Persistence.Repositories
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly VoiceAgentDbContext _db;

        public UnitOfWork(VoiceAgentDbContext db)
        {
            _db = db;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }
    }
}
