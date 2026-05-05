using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Contracts.Conversations;

namespace VoiceAgentRag.Application.Conversations
{
    public interface IConversationQueryService
    {
        Task<ConversationHistoryDto?> GetHistoryAsync(
            Guid conversationId,
            CancellationToken cancellationToken = default);
    }
}
