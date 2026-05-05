using System.Numerics;
using VoiceAgentRag.Domain.Common;

namespace VoiceAgentRag.Domain.Knowledge
{
    public sealed class KnowledgeChunk : Entity
    {
        private KnowledgeChunk() { }

        public KnowledgeChunk(Guid documentId, int index, string content, string? language = null)
        {
            if (documentId == Guid.Empty)
                throw new ArgumentException("Document id is required.");

            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Chunk content is required.");

            DocumentId = documentId;
            Index = index;
            Content = content.Trim();

            Language = Languages.IsSupported(language ?? "")
                ? language!
                : Languages.French;
        }

        public void SetEmbedding(float[] embedding)
        {
            if (embedding is null || embedding.Length == 0)
                throw new ArgumentException("Embedding cannot be empty.", nameof(embedding));

            Embedding = embedding;
        }

        public Guid DocumentId { get; private set; }
        public int Index { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public string Language { get; private set; } = Languages.French;

        public float[]? Embedding { get; private set; }
    }
}
