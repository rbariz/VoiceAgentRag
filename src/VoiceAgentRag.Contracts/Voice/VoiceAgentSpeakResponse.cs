namespace VoiceAgentRag.Contracts.Voice
{
    public sealed record VoiceAgentSpeakResponse(
    Guid ConversationId,
    string Language,
    string Transcription,
    string Intent,
    string Answer,
    bool RequiresHumanHandoff,
    string AudioContentType,
    string AudioBase64);
}
