using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Application.Abstractions.AI;
using VoiceAgentRag.Application.Abstractions.Audio;
using VoiceAgentRag.Application.Abstractions.Persistence;
using VoiceAgentRag.Application.Abstractions.Rag;
using VoiceAgentRag.Infrastructure.AI;
using VoiceAgentRag.Infrastructure.AI.Ollama;
using VoiceAgentRag.Infrastructure.Audio;
using VoiceAgentRag.Infrastructure.Options;
using VoiceAgentRag.Infrastructure.Persistence;
using VoiceAgentRag.Infrastructure.Persistence.Repositories;
using VoiceAgentRag.Infrastructure.Rag;
using static VoiceAgentRag.Infrastructure.Audio.FakeTextToSpeechService;

namespace VoiceAgentRag.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

            services.AddDbContext<VoiceAgentDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });


            services.Configure<AiProviderOptions>(
    configuration.GetSection("AiProviders"));

            var aiOptions = configuration
    .GetSection("AiProviders")
    .Get<AiProviderOptions>() ?? new AiProviderOptions();

            if (!string.Equals(aiOptions.LlmProvider, "Fake", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(aiOptions.LlmProvider, "Ollama", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Supported LLM providers: Fake, Ollama.");
            }

            //if (!string.Equals(aiOptions.SpeechToTextProvider, "Fake", StringComparison.OrdinalIgnoreCase))
            //    throw new InvalidOperationException("Only Fake Speech-to-Text provider is supported in the MVP.");

            if (!string.Equals(aiOptions.TextToSpeechProvider, "Fake", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only Fake Text-to-Speech provider is supported in the MVP.");

            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddScoped<ITextChunker, SimpleTextChunker>();
            services.AddScoped<IRagService, SimpleRagService>();

            services.AddScoped<IIntentDetector, SimpleIntentDetector>();

            if (string.Equals(aiOptions.LlmProvider, "Ollama", StringComparison.OrdinalIgnoreCase))
            {
                services.AddHttpClient<IAnswerGenerator, OllamaAnswerGenerator>(client =>
                {
                    client.BaseAddress = new Uri(aiOptions.OllamaBaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(aiOptions.OllamaTimeoutSeconds);
                });
            }
            else
            {
                services.AddScoped<IAnswerGenerator, SimpleRagAnswerGenerator>();
            }

            var useOpenAi = string.Equals(
                    aiOptions.SpeechToTextProvider,
                    "OpenAIWhisper",
                    StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace(aiOptions.OpenAiApiKey)
                    && aiOptions.OpenAiApiKey != "YOUR_KEY";

            if (useOpenAi)
            {
                services.AddHttpClient<ISpeechToTextService, OpenAiWhisperSpeechToTextService>(client =>
                {
                    client.BaseAddress = new Uri(aiOptions.OpenAiBaseUrl);
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", aiOptions.OpenAiApiKey);
                });

                Console.WriteLine("STT Provider: OpenAI Whisper");
            }
            else
            {
                services.AddScoped<ISpeechToTextService, FakeSpeechToTextService>();

                Console.WriteLine("STT Provider: Fake (no valid API key)");
            }
            services.AddScoped<ITextToSpeechService, FakeTextToSpeechService>();

            services.AddScoped<IVoiceInteractionRepository, VoiceInteractionRepository>();

            services.AddHttpClient<IEmbeddingGenerator, OllamaEmbeddingGenerator>(client =>
            {
                client.BaseAddress = new Uri(aiOptions.OllamaBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(aiOptions.OllamaTimeoutSeconds);
            });

            services.AddHttpClient<IStreamingAnswerGenerator, OllamaStreamingAnswerGenerator>(client =>
            {
                client.BaseAddress = new Uri(aiOptions.OllamaBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(aiOptions.OllamaTimeoutSeconds);
            });

            return services;
        }
    }
}
