namespace VoiceAgentRag.Contracts.Conversations
{
    public sealed record ConversationHistoryDto(
        Guid Id,
        string? CustomerReference,
        string Language,
        string Status,
        DateTime CreatedAtUtc,
        IReadOnlyList<ConversationMessageDto> Messages,
        IReadOnlyList<VoiceInteractionDto> VoiceInteractions);

}
