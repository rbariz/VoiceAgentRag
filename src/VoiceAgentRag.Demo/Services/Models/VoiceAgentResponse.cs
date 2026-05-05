namespace VoiceAgentRag.Demo.Services.Models
{
    public sealed record VoiceAgentResponse(
    Guid ConversationId,
    string Language,
    string Transcription,
    string Intent,
    string Answer,
    bool RequiresHumanHandoff);
}
