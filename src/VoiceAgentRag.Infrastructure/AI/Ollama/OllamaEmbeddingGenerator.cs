using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using VoiceAgentRag.Application.Abstractions.AI;
using VoiceAgentRag.Infrastructure.Options;

namespace VoiceAgentRag.Infrastructure.AI.Ollama
{
    public sealed class OllamaEmbeddingGenerator : IEmbeddingGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly AiProviderOptions _options;
        private readonly ILogger<OllamaEmbeddingGenerator> _logger;

        public OllamaEmbeddingGenerator(
            HttpClient httpClient,
            IOptions<AiProviderOptions> options,
            ILogger<OllamaEmbeddingGenerator> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<float[]> GenerateEmbeddingAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text is required.", nameof(text));

            var request = new OllamaEmbedRequest(
                _options.OllamaEmbeddingModel,
                text);

            using var response = await _httpClient.PostAsJsonAsync(
                "/api/embed",
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Ollama embedding failed with status {StatusCode}", response.StatusCode);
                throw new InvalidOperationException("Ollama embedding generation failed.");
            }

            var result = await response.Content.ReadFromJsonAsync<OllamaEmbedResponse>(
                cancellationToken: cancellationToken);

            var embedding = result?.Embeddings?.FirstOrDefault();

            if (embedding is null || embedding.Length == 0)
                throw new InvalidOperationException("Ollama returned empty embedding.");

            if (embedding.Length != _options.EmbeddingDimensions)
            {
                throw new InvalidOperationException(
                    $"Embedding dimension mismatch. Expected {_options.EmbeddingDimensions}, got {embedding.Length}.");
            }

            return embedding;
        }
    }
}
