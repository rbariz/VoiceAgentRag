using VoiceAgentRag.Application.Abstractions.Persistence;
using VoiceAgentRag.Application.Abstractions.Rag;
using VoiceAgentRag.Contracts.Knowledge;
using VoiceAgentRag.Domain.Common;
using VoiceAgentRag.Domain.Knowledge;

namespace VoiceAgentRag.Application.Knowledge
{
    public sealed class KnowledgeIngestionService : IKnowledgeIngestionService
    {
        private readonly IKnowledgeRepository _knowledgeRepository;
        private readonly ITextChunker _chunker;
        private readonly IUnitOfWork _unitOfWork;

        public KnowledgeIngestionService(
            IKnowledgeRepository knowledgeRepository,
            ITextChunker chunker,
            IUnitOfWork unitOfWork)
        {
            _knowledgeRepository = knowledgeRepository;
            _chunker = chunker;
            _unitOfWork = unitOfWork;
        }

        public async Task<IngestDocumentResponse> IngestAsync(
            IngestDocumentRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Title is required.", nameof(request));

            if (string.IsNullOrWhiteSpace(request.Source))
                throw new ArgumentException("Source is required.", nameof(request));

            if (string.IsNullOrWhiteSpace(request.Content))
                throw new ArgumentException("Content is required.", nameof(request));

            var language = Languages.IsSupported(request.Language)
                ? request.Language
                : Languages.French;

            var document = new KnowledgeDocument(
                request.Title,
                request.Source,
                request.Content,
                language);

            var chunks = _chunker
                .Split(request.Content)
                .Select((content, index) => new KnowledgeChunk(
                    document.Id,
                    index,
                    content,
                    language))
                .ToList();

            _knowledgeRepository.AddDocument(document);
            _knowledgeRepository.AddChunks(chunks);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new IngestDocumentResponse(
                document.Id,
                chunks.Count);
        }
    }
}
