using System.Runtime.CompilerServices;
using System.Text.Json;
using VoiceAgentRag.Demo.Services.Models;

namespace VoiceAgentRag.Demo.Services
{

    public sealed class VoiceAgentApiClient
    {
        private readonly HttpClient _httpClient;

        public VoiceAgentApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<VoiceAgentResponse> AskAsync(
            VoiceAgentTextRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/api/voice-agent/text",
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException(
                        $"XXXX API error {(int)response.StatusCode}: {error}");
            }

            return await response.Content.ReadFromJsonAsync<VoiceAgentResponse>(
                cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException("Empty API response.");
        }



        public async IAsyncEnumerable<VoiceAgentStreamEvent> AskStreamAsync(
    VoiceAgentTextRequest request,
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/voice-agent/text/stream")
            {
                Content = JsonContent.Create(request)
            };

            using var response = await _httpClient.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"API error {(int)response.StatusCode}: {error}");
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(cancellationToken);

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var item = JsonSerializer.Deserialize<VoiceAgentStreamEvent>(
                    line,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (item is not null)
                    yield return item;
            }
        }
    }
}
