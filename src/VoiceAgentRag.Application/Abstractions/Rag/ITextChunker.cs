namespace VoiceAgentRag.Application.Abstractions.Rag
{
    public interface ITextChunker
    {
        IReadOnlyList<string> Split(string text, int maxChunkLength = 1200);
    }


}
