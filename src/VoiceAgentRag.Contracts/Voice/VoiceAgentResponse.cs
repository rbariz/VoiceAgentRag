namespace VoiceAgentRag.Contracts.Voice
{
    public sealed record VoiceAgentResponse(
    Guid ConversationId,
    string Transcription,
    string Intent,
    string Answer,
    bool RequiresHumanHandoff);
}
