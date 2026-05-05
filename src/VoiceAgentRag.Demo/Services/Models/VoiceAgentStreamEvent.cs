namespace VoiceAgentRag.Demo.Services.Models
{
    public sealed record VoiceAgentStreamEvent(
    string Type,
    Guid? ConversationId,
    string? Language,
    string? Intent,
    bool? RequiresHumanHandoff,
    string? Token);
}
