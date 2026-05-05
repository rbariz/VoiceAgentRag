using Microsoft.EntityFrameworkCore;
using VoiceAgentRag.Application.Abstractions.Persistence;
using VoiceAgentRag.Domain.Voice;

namespace VoiceAgentRag.Infrastructure.Persistence.Repositories
{
    public sealed class VoiceInteractionRepository : IVoiceInteractionRepository
    {
        private readonly VoiceAgentDbContext _db;

        public VoiceInteractionRepository(VoiceAgentDbContext db)
        {
            _db = db;
        }

        public void Add(VoiceInteraction interaction)
        {
            _db.VoiceInteractions.Add(interaction);
        }

        public async Task<IReadOnlyList<VoiceInteraction>> GetByConversationIdAsync(
            Guid conversationId,
            CancellationToken cancellationToken = default)
        {
            return await _db.VoiceInteractions
                .Where(x => x.ConversationId == conversationId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync(cancellationToken);
        }
    }
}
