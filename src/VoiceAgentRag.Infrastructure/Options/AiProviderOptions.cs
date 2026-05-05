using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAgentRag.Infrastructure.Options
{
    public sealed class AiProviderOptions
    {
        public string LlmProvider { get; set; } = "Fake";
        public string SpeechToTextProvider { get; set; } = "Fake";
        public string TextToSpeechProvider { get; set; } = "Fake";

        public string? OpenAiApiKey { get; set; }
        public string? OpenAiModel { get; set; } = "gpt-4o-mini";

        public string? AzureSpeechKey { get; set; }
        public string? AzureSpeechRegion { get; set; }

        public string? ElevenLabsApiKey { get; set; }
        public string? ElevenLabsVoiceId { get; set; }
        //
        public string OllamaBaseUrl { get; set; } = "http://localhost:11434";
        public string OllamaModel { get; set; } = "llama3.2";
        public int OllamaTimeoutSeconds { get; set; } = 120;
        public double OllamaTemperature { get; set; } = 0.2;

        public string EmbeddingProvider { get; set; } = "Ollama";
        public string OllamaEmbeddingModel { get; set; } = "nomic-embed-text";
        public int EmbeddingDimensions { get; set; } = 768;
    }
}
