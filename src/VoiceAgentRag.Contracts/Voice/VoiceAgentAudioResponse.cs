namespace VoiceAgentRag.Contracts.Voice
{
    public sealed record VoiceAgentAudioResponse(
    Guid ConversationId,
    string Language,
    string Transcription,
    string Intent,
    string Answer,
    bool RequiresHumanHandoff);
}
