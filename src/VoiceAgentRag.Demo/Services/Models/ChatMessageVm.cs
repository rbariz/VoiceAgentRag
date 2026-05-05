namespace VoiceAgentRag.Demo.Services.Models
{
    public sealed record ChatMessageVm(
    string Role,
    string Content,
    DateTime CreatedAtUtc);
}
