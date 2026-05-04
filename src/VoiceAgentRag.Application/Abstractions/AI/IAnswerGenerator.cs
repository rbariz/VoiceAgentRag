namespace VoiceAgentRag.Application.Abstractions.AI
{
    public interface IAnswerGenerator
    {
        Task<string> GenerateAnswerAsync(
            string userText,
            string intent,
            string language,
            IReadOnlyList<string> contextChunks,
            CancellationToken cancellationToken = default);
    }
}
