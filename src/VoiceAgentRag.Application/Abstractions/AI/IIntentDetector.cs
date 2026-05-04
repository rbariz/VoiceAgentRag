namespace VoiceAgentRag.Application.Abstractions.AI
{
    public interface IIntentDetector
    {
        Task<IntentResult> DetectAsync(
            string userText,
            string language,
            CancellationToken cancellationToken = default);
    }
}
