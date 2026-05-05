using System.Text.Json.Serialization;

namespace VoiceAgentRag.Infrastructure.AI.Ollama
{
    public sealed record OllamaEmbedRequest(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("input")] string Input);
}
