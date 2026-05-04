namespace VoiceAgentRag.Contracts.Conversations
{
    public sealed record ConversationMessageDto(
    Guid Id,
    Guid ConversationId,
    string Role,
    string Content,
    DateTime CreatedAtUtc);

}
