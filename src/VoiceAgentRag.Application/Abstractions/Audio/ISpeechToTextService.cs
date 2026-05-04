namespace VoiceAgentRag.Application.Abstractions.Audio
{
    public interface ISpeechToTextService
    {
        Task<SpeechToTextResult> TranscribeAsync(
            Stream audioStream,
            string? language,
            CancellationToken cancellationToken = default);
    }
}
