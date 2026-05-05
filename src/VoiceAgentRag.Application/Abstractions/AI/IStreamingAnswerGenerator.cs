namespace VoiceAgentRag.Application.Abstractions.AI
{
    public interface IStreamingAnswerGenerator
    {
        IAsyncEnumerable<string> GenerateAnswerStreamAsync(
            string userText,
            string intent,
            string language,
            IReadOnlyList<string> contextChunks,
            CancellationToken cancellationToken = default);
    }
}
