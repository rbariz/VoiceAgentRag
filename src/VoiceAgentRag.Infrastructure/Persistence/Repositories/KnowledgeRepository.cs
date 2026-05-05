using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VoiceAgentRag.Application.Abstractions.Persistence;
using VoiceAgentRag.Domain.Knowledge;

namespace VoiceAgentRag.Infrastructure.Persistence.Repositories
{
    public sealed class KnowledgeRepository : IKnowledgeRepository
    {
        private readonly VoiceAgentDbContext _db;

        public KnowledgeRepository(VoiceAgentDbContext db)
        {
            _db = db;
        }

        public void AddDocument(KnowledgeDocument document)
        {
            _db.KnowledgeDocuments.Add(document);
        }

        public void AddChunks(IEnumerable<KnowledgeChunk> chunks)
        {
            _db.KnowledgeChunks.AddRange(chunks);
        }

        public async Task<IReadOnlyList<KnowledgeChunk>> SearchRelevantChunksAsync(
    string query,
    string language,
    int maxResults,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                return [];

            var terms = query
                .ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => x.Length >= 3)
                .Distinct()
                .ToList();

            if (terms.Count == 0)
                return [];

            var chunks = await _db.KnowledgeChunks
                .Where(x => x.Language == language)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(200)
                .ToListAsync(cancellationToken);

            return chunks
                .Select(chunk => new
                {
                    Chunk = chunk,
                    Score = ScoreChunk(chunk.Content, terms)
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Chunk.CreatedAtUtc)
                .Take(maxResults)
                .Select(x => x.Chunk)
                .ToList();
        }

        public async Task UpdateChunkEmbeddingAsync(
    Guid chunkId,
    float[] embedding,
    CancellationToken cancellationToken = default)
        {
            var vector = ToPgVectorLiteral(embedding);

            await _db.Database.ExecuteSqlRawAsync(
                """
        UPDATE knowledge_chunks
        SET embedding = {0}::vector
        WHERE id = {1}
        """,
                [vector, chunkId],
                cancellationToken);
        }

        public async Task<IReadOnlyList<KnowledgeChunk>> SearchSimilarChunksAsync(
            float[] queryEmbedding,
            string language,
            int maxResults,
            CancellationToken cancellationToken = default)
        {
            var vector = ToPgVectorLiteral(queryEmbedding);

            return await _db.KnowledgeChunks
                .FromSqlRaw(
                    """
            SELECT id, document_id, chunk_index, content, language, created_at_utc
            FROM knowledge_chunks
            WHERE language = {0}
              AND embedding IS NOT NULL
            ORDER BY embedding <=> {1}::vector
            LIMIT {2}
            """,
                    language,
                    vector,
                    maxResults)
                .ToListAsync(cancellationToken);
        }

        private static string ToPgVectorLiteral(float[] values)
        {
            var formatted = values.Select(x =>
                x.ToString("G9", CultureInfo.InvariantCulture));

            return $"[{string.Join(",", formatted)}]";
        }
        private static int ScoreChunk(string content, IReadOnlyList<string> terms)
        {
            if (string.IsNullOrWhiteSpace(content))
                return 0;

            var normalized = content.ToLowerInvariant();
            var score = 0;

            foreach (var term in terms)
            {
                if (normalized.Contains(term))
                    score += 1;
            }

            return score;
        }
    }
}
