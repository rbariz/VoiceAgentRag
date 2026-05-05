namespace VoiceAgentRag.Contracts.Conversations
{
    public sealed record VoiceInteractionDto(
        Guid Id,
        Guid ConversationId,
        string Language,
        string? AudioInputPath,
        string Transcription,
        string ResponseText,
        string? AudioOutputPath,
        DateTime CreatedAtUtc);

}
