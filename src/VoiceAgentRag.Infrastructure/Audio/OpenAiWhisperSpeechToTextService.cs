using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using VoiceAgentRag.Application.Abstractions.Audio;
using VoiceAgentRag.Domain.Common;
using VoiceAgentRag.Infrastructure.Options;

namespace VoiceAgentRag.Infrastructure.Audio
{
    public sealed partial class FakeTextToSpeechService
    {
        public sealed class OpenAiWhisperSpeechToTextService : ISpeechToTextService
    {
        private readonly HttpClient _httpClient;
        private readonly AiProviderOptions _options;
        private readonly ILogger<OpenAiWhisperSpeechToTextService> _logger;
        public OpenAiWhisperSpeechToTextService(
            HttpClient httpClient,
            IOptions<AiProviderOptions> options,
            ILogger<OpenAiWhisperSpeechToTextService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<SpeechToTextResult> TranscribeAsync(
            Stream audioStream,
            string? language,
            CancellationToken cancellationToken = default)
        {
            var lang = Languages.IsSupported(language ?? "")
                ? language!
                : Languages.French;

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(_options.OpenAiTranscriptionModel), "model");
            content.Add(new StringContent(lang), "language");

            var audioContent = new StreamContent(audioStream);
            audioContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");

            content.Add(audioContent, "file", "audio.mp3");

            using var response = await _httpClient.PostAsync(
                "/v1/audio/transcriptions",
                content,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                    //var error = await response.Content.ReadAsStringAsync(cancellationToken);
                    //throw new InvalidOperationException($"OpenAI transcription failed: {error}");
                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync(cancellationToken);

                        _logger.LogWarning("Whisper failed, fallback to fake. Error: {Error}", error);

                        return new SpeechToTextResult(
                            "Je veux suivre ma commande",
                            "fr",
                            null);
                    }
                }

            var result = await response.Content.ReadFromJsonAsync<OpenAiTranscriptionResponse>(
                cancellationToken: cancellationToken);

            return new SpeechToTextResult(
                result?.Text ?? "",
                lang,
                Confidence: null);
        }

        private sealed record OpenAiTranscriptionResponse(string Text);
    }
}
}
