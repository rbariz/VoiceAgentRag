namespace VoiceAgentRag.Contracts.Knowledge
{
    public sealed record IngestDocumentResponse(
    Guid DocumentId,
    int ChunkCount);
}
