using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Application.Abstractions.Persistence;
using VoiceAgentRag.Domain.Conversations;

namespace VoiceAgentRag.Infrastructure.Persistence.Repositories
{
    public sealed class ConversationRepository : IConversationRepository
    {
        private readonly VoiceAgentDbContext _db;

        public ConversationRepository(VoiceAgentDbContext db)
        {
            _db = db;
        }

        public Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _db.Conversations
                .Include(x => x.Messages)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public void Add(Conversation conversation)
        {
            _db.Conversations.Add(conversation);
        }
    }
}
