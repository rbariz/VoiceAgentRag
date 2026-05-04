using VoiceAgentRag.Domain.Common;

namespace VoiceAgentRag.Domain.Knowledge
{
    public sealed class KnowledgeChunk : Entity
    {
        private KnowledgeChunk() { }

        public KnowledgeChunk(Guid documentId, int index, string content, string? language = null)
        {
            if (documentId == Guid.Empty)
                throw new ArgumentException("Document id is required.", nameof(documentId));

            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Chunk content is required.", nameof(content));

            DocumentId = documentId;
            Index = index;
            Content = content.Trim();

            Language = Languages.IsSupported(language ?? "")
                ? language!
                : Languages.French;
        }

        public Guid DocumentId { get; private set; }
        public int Index { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public string Language { get; private set; } = Languages.French;
    }
}
