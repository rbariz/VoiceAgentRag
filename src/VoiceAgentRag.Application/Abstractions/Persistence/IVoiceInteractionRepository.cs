using VoiceAgentRag.Domain.Voice;

namespace VoiceAgentRag.Application.Abstractions.Persistence
{
    public interface IVoiceInteractionRepository
    {
        void Add(VoiceInteraction interaction);

        Task<IReadOnlyList<VoiceInteraction>> GetByConversationIdAsync(
            Guid conversationId,
            CancellationToken cancellationToken = default);
    }
}
