using VoiceAgentRag.Domain.Conversations;

namespace VoiceAgentRag.Application.Abstractions.Persistence
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        void Add(Conversation conversation);

        void AddMessage(ConversationMessage message);
    }
}
