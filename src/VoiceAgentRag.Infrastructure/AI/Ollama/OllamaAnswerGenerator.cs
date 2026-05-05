using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VoiceAgentRag.Application.Abstractions.AI;
using VoiceAgentRag.Infrastructure.Options;

namespace VoiceAgentRag.Infrastructure.AI.Ollama;

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
        if (string.IsNullOrWhiteSpace(userText))
            return BuildFallbackAnswer(language);

        if (contextChunks.Count == 0)
            return BuildNoContextAnswer(language);

        var request = new OllamaGenerateRequest(
            Model: _options.OllamaModel,
            System: BuildSystemPrompt(language),
            Prompt: BuildUserPrompt(userText, intent, language, contextChunks),
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

            return string.IsNullOrWhiteSpace(result?.Response)
                ? BuildFallbackAnswer(language)
                : result.Response.Trim();
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
أنت وكيل خدمة عملاء صوتي محترف.

الدور:
- أجب على أسئلة العملاء بوضوح ولطف.
- استخدم فقط السياق المقدم.
- لا تخترع أي معلومات.
- اجعل الإجابة قصيرة ومناسبة للمحادثة الصوتية.

القواعد:
- إذا كانت الإجابة موجودة في السياق، استخدمها.
- إذا لم تكن الإجابة موجودة في السياق، قل إنك لا تملك معلومات كافية.
- إذا كان الطلب حساساً أو غير واضح، اقترح تحويل العميل إلى موظف دعم.
- لا تتجاوز 3 أو 4 جمل.
- أجب دائماً بالعربية.
""",

            "en" => """
You are a professional customer service voice assistant.

Role:
- Answer customer questions clearly and politely.
- Use only the provided context.
- Do not invent information.
- Keep the answer short and voice-friendly.

Rules:
- If the answer is in the context, use it.
- If the answer is not in the context, say that you do not have enough information.
- If the request is sensitive or unclear, offer human handoff.
- Do not exceed 3 or 4 sentences.
- Always answer in English.
""",

            _ => """
Tu es un assistant vocal professionnel de service client.

Rôle :
- Répondre clairement et poliment aux questions des clients.
- Utiliser uniquement le contexte fourni.
- Ne jamais inventer d’informations.
- Garder une réponse courte et adaptée à une conversation vocale.

Règles :
- Si la réponse est dans le contexte, utilise-la.
- Si la réponse n’est pas dans le contexte, dis que tu n’as pas assez d’informations.
- Si la demande est sensible ou ambiguë, propose un transfert vers un conseiller humain.
- Ne dépasse pas 3 ou 4 phrases.
- Réponds toujours en français.
"""
        };
    }

    private static string BuildUserPrompt(
        string userText,
        string intent,
        string language,
        IReadOnlyList<string> contextChunks)
    {
        var context = string.Join("\n\n---\n\n", contextChunks.Take(5));

        return $"""
Language: {language}
Detected intent: {intent}

Knowledge base context:
{context}

Customer question:
{userText}

Expected answer:
""";
    }

    private static string BuildNoContextAnswer(string language)
    {
        return language switch
        {
            "ar" => "لا أملك معلومات كافية للإجابة على سؤالك بدقة. هل ترغب في التواصل مع موظف دعم؟",
            "en" => "I don’t have enough information to answer your question accurately. Would you like to be connected to a human agent?",
            _ => "Je n’ai pas suffisamment d’informations pour répondre précisément à votre question. Souhaitez-vous être mis en relation avec un conseiller ?"
        };
    }

    private static string BuildFallbackAnswer(string language)
    {
        return language switch
        {
            "ar" => "لا أستطيع معالجة الطلب حالياً. يمكنني تحويلك إلى موظف دعم.",
            "en" => "I cannot process the request right now. I can route you to a human agent.",
            _ => "Je ne peux pas traiter la demande pour le moment. Je peux vous transférer vers un conseiller humain."
        };
    }
}