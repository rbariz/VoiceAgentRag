namespace VoiceAgentRag.Application.Abstractions.Audio
{
    public interface ITextToSpeechService
    {
        Task<TextToSpeechResult> SynthesizeAsync(
            string text,
            string language,
            CancellationToken cancellationToken = default);
    }
}
