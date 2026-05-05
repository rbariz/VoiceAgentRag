using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using VoiceAgentRag.Application.Abstractions.AI;
using VoiceAgentRag.Infrastructure.Options;

namespace VoiceAgentRag.Infrastructure.AI.Ollama
{
    public sealed class OllamaAnswerGenerator : IAnswerGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly AiProviderOptions _options;
        private readonly ILogger<OllamaAnswerGenerator> _logger;

        public OllamaAnswerGenerator(
            HttpClient httpClient,
            IOptions<AiProviderOptions> options,
            ILogger<OllamaAnswerGenerator> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<string> GenerateAnswerAsync(
            string userText,
            string intent,
            string language,
            IReadOnlyList<string> contextChunks,
            CancellationToken cancellationToken = default)
        {
            var system = BuildSystemPrompt(language);
            var prompt = BuildUserPrompt(userText, intent, language, contextChunks);

            var request = new OllamaGenerateRequest(
                Model: _options.OllamaModel,
                System: system,
                Prompt: prompt,
                Stream: false,
                Options: new OllamaGenerateOptions(_options.OllamaTemperature));

            try
            {
                _logger.LogInformation(
    "Using Ollama provider. Model={Model}, BaseUrl={BaseUrl}",
    _options.OllamaModel,
    _options.OllamaBaseUrl);
                using var response = await _httpClient.PostAsJsonAsync(
                    "/api/generate",
                    request,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Ollama returned non-success status code {StatusCode}",
                        response.StatusCode);

                    return BuildFallbackAnswer(language);
                }

                var result = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>(
                    cancellationToken: cancellationToken);

                if (string.IsNullOrWhiteSpace(result?.Response))
                    return BuildFallbackAnswer(language);

                return result.Response.Trim();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ollama answer generation failed");
                return BuildFallbackAnswer(language);
            }
        }

        private static string BuildSystemPrompt(string language)
        {
            return language switch
            {
                "ar" => """
                        أنت وكيل خدمة عملاء صوتي.
                        أجب فقط اعتماداً على السياق المقدم.
                        إذا لم تجد المعلومة في السياق، قل إنك لا تملك معلومات كافية واقترح تحويل الطلب إلى موظف دعم.
                        استخدم لغة عربية واضحة ومختصرة.
                        لا تخترع معلومات.
                        """,
                                        "en" => """
                        You are a customer service voice agent.
                        Answer only using the provided context.
                        If the answer is not in the context, say that you do not have enough information and offer human handoff.
                        Use clear and concise English.
                        Do not invent information.
                        """,
                                        _ => """
                        Tu es un agent vocal de service client.
                        Réponds uniquement à partir du contexte fourni.
                        Si la réponse n’est pas dans le contexte, dis que tu n’as pas assez d’informations et propose un transfert vers un conseiller humain.
                        Utilise un français clair et concis.
                        N’invente pas d’informations.
                        """
            };
        }

        private static string BuildUserPrompt(
            string userText,
            string intent,
            string language,
            IReadOnlyList<string> contextChunks)
        {
            var context = contextChunks.Count == 0
                ? "NO_CONTEXT"
                : string.Join("\n\n---\n\n", contextChunks.Take(5));

            return $"""
                    Language: {language}
                    Intent: {intent}

                    Context:
                    {context}

                    Customer question:
                    {userText}

                Answer:
                """;
                        }

        private static string BuildFallbackAnswer(string language)
        {
            return language switch
            {
                "ar" => "لا أملك معلومات كافية للإجابة بدقة. يمكنني تحويل طلبك إلى موظف دعم.",
                "en" => "I do not have enough information to answer accurately. I can route this request to a human agent.",
                _ => "Je n’ai pas assez d’informations pour répondre avec précision. Je peux transférer votre demande à un conseiller humain."
            };
        }
    }
}
