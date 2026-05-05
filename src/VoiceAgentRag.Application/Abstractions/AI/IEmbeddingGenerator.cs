namespace VoiceAgentRag.Application.Abstractions.AI
{
    public interface IEmbeddingGenerator
    {
        Task<float[]> GenerateEmbeddingAsync(
            string text,
            CancellationToken cancellationToken = default);
    }
}
