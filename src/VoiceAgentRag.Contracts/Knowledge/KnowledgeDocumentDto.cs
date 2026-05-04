namespace VoiceAgentRag.Contracts.Knowledge
{
    public sealed record KnowledgeDocumentDto(
    Guid Id,
    string Title,
    string Source,
    DateTime CreatedAtUtc);
}
