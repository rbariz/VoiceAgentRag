using VoiceAgentRag.Application.Abstractions.Persistence;
using VoiceAgentRag.Contracts.Conversations;

namespace VoiceAgentRag.Application.Conversations
{
    public sealed class ConversationQueryService : IConversationQueryService
    {
        private readonly IConversationRepository _conversations;
        private readonly IVoiceInteractionRepository _voiceInteractions;

        public ConversationQueryService(
            IConversationRepository conversations,
            IVoiceInteractionRepository voiceInteractions)
        {
            _conversations = conversations;
            _voiceInteractions = voiceInteractions;
        }

        public async Task<ConversationHistoryDto?> GetHistoryAsync(
            Guid conversationId,
            CancellationToken cancellationToken = default)
        {
            var conversation = await _conversations.GetByIdAsync(
                conversationId,
                cancellationToken);

            if (conversation is null)
                return null;

            var interactions = await _voiceInteractions.GetByConversationIdAsync(
                conversationId,
                cancellationToken);

            return new ConversationHistoryDto(
                conversation.Id,
                conversation.CustomerReference,
                conversation.Language,
                conversation.Status.ToString(),
                conversation.CreatedAtUtc,
                conversation.Messages
                    .OrderBy(x => x.CreatedAtUtc)
                    .Select(x => new ConversationMessageDto(
                        x.Id,
                        x.ConversationId,
                        x.Role.ToString(),
                        x.Content,
                        x.CreatedAtUtc))
                    .ToList(),
                interactions
                    .OrderBy(x => x.CreatedAtUtc)
                    .Select(x => new VoiceInteractionDto(
                        x.Id,
                        x.ConversationId,
                        x.Language,
                        x.AudioInputPath,
                        x.Transcription,
                        x.ResponseText,
                        x.AudioOutputPath,
                        x.CreatedAtUtc))
                    .ToList());
        }
    }
}
