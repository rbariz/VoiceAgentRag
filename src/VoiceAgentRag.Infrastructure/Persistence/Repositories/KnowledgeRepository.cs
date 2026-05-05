using Microsoft.EntityFrameworkCore;
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

            var normalizedQuery = query.Trim().ToLower();

            return await _db.KnowledgeChunks
                .Where(x => x.Language == language)
                .Where(x => x.Content.ToLower().Contains(normalizedQuery))
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(maxResults)
                .ToListAsync(cancellationToken);
        }
    }
}
