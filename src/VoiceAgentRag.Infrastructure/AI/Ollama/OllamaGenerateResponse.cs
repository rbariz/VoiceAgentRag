using System.Text.Json.Serialization;

namespace VoiceAgentRag.Infrastructure.AI.Ollama
{
    public sealed record OllamaGenerateResponse(
    [property: JsonPropertyName("response")] string? Response,
    [property: JsonPropertyName("done")] bool Done);
}
