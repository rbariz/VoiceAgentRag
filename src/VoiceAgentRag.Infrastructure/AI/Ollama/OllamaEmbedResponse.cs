using System.Text.Json.Serialization;

namespace VoiceAgentRag.Infrastructure.AI.Ollama
{
    public sealed record OllamaEmbedResponse(
        [property: JsonPropertyName("embeddings")] float[][]? Embeddings);
}
