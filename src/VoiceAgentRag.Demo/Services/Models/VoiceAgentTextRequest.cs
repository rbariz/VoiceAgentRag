namespace VoiceAgentRag.Demo.Services.Models
{
    public sealed record VoiceAgentTextRequest(
    Guid? ConversationId,
    string UserText,
    string? Language,
    string? CustomerReference);
}
