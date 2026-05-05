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
    }
}
