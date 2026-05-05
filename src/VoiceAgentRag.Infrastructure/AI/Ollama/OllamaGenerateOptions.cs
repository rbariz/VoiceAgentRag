using System.Text.Json.Serialization;

namespace VoiceAgentRag.Infrastructure.AI.Ollama
{
    public sealed record OllamaGenerateOptions(
    [property: JsonPropertyName("temperature")] double Temperature);
}
