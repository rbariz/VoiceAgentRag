using VoiceAgentRag.Application.Abstractions.AI;
using VoiceAgentRag.Application.Abstractions.Persistence;
using VoiceAgentRag.Application.Abstractions.Rag;

namespace VoiceAgentRag.Infrastructure.Rag
{
    public sealed class SimpleRagService : IRagService
    {
        private readonly IKnowledgeRepository _knowledgeRepository;
        private readonly IEmbeddingGenerator _embeddingGenerator;

        public SimpleRagService(IKnowledgeRepository knowledgeRepository, IEmbeddingGenerator embeddingGenerator)
        {
            _knowledgeRepository = knowledgeRepository;
            _embeddingGenerator = embeddingGenerator;
        }

        public async Task<IReadOnlyList<string>> RetrieveContextAsync(
    string query,
    string language,
    int maxChunks = 5,
    CancellationToken cancellationToken = default)
        {
            // 1) Vector search
            var embedding = await _embeddingGenerator.GenerateEmbeddingAsync(
                $"search_query: {query}",
                cancellationToken);

            var vectorResults = await _knowledgeRepository.SearchSimilarChunksAsync(
                embedding,
                language,
                10,
                cancellationToken);

            // 2) Keyword search
            var keywordResults = await _knowledgeRepository.SearchRelevantChunksAsync(
                query,
                language,
                10,
                cancellationToken);

            // 3) Merge + scoring
            var combined = vectorResults
                .Concat(keywordResults)
                .GroupBy(x => x.Id)
                .Select(g =>
                {
                    var chunk = g.First();

                    var vectorRank = vectorResults
                    .Select((x, index) => new { x.Id, index })
                    .FirstOrDefault(x => x.Id == chunk.Id)?.index ?? -1;

                    var keywordScore = KeywordScore(chunk.Content, query);

                    var score =
                        (vectorRank >= 0 ? (10 - vectorRank) : 0) +
                        (keywordScore * 2);

                    return new
                    {
                        chunk,
                        score
                    };
                })
                .OrderByDescending(x => x.score)
                .Take(maxChunks)
                .Select(x => x.chunk.Content)
                .ToList();

            return combined;
        }


        private static int KeywordScore(string content, string query)
        {
            var terms = query
                .ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var normalized = content.ToLowerInvariant();

            var score = 0;

            foreach (var term in terms)
            {
                if (term.Length >= 3 && normalized.Contains(term))
                    score++;
            }

            return score;
        }
    }
}
