using VoiceAgentRag.Application.Abstractions.Persistence;
using VoiceAgentRag.Application.Abstractions.Rag;

namespace VoiceAgentRag.Infrastructure.Rag
{
    public sealed class SimpleRagService : IRagService
    {
        private readonly IKnowledgeRepository _knowledgeRepository;

        public SimpleRagService(IKnowledgeRepository knowledgeRepository)
        {
            _knowledgeRepository = knowledgeRepository;
        }

        public async Task<IReadOnlyList<string>> RetrieveContextAsync(
            string query,
            string language,
            int maxChunks = 5,
            CancellationToken cancellationToken = default)
        {
            var chunks = await _knowledgeRepository.SearchRelevantChunksAsync(
                query,
                language,
                maxChunks,
                cancellationToken);

            return chunks
                .Select(x => x.Content)
                .ToList();
        }
    }
}
