using VoiceAgentRag.Domain.Knowledge;

namespace VoiceAgentRag.Application.Abstractions.Persistence
{
    public interface IKnowledgeRepository
    {
        void AddDocument(KnowledgeDocument document);
        void AddChunks(IEnumerable<KnowledgeChunk> chunks);

        Task<IReadOnlyList<KnowledgeChunk>> SearchRelevantChunksAsync(
            string query,
            string language,
            int maxResults,
            CancellationToken cancellationToken = default);
    }
}
