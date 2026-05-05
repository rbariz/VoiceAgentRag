using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using VoiceAgentRag.Application.Abstractions.AI;
using VoiceAgentRag.Infrastructure.Options;

namespace VoiceAgentRag.Infrastructure.AI.Ollama;

public sealed class OllamaStreamingAnswerGenerator : IStreamingAnswerGenerator
{
    private readonly HttpClient _httpClient;
    private readonly AiProviderOptions _options;
    private readonly ILogger<OllamaStreamingAnswerGenerator> _logger;

    public OllamaStreamingAnswerGenerator(
        HttpClient httpClient,
        IOptions<AiProviderOptions> options,
        ILogger<OllamaStreamingAnswerGenerator> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async IAsyncEnumerable<string> GenerateAnswerStreamAsync(
        string userText,
        string intent,
        string language,
        IReadOnlyList<string> contextChunks,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (contextChunks.Count == 0)
        {
            yield return BuildNoContextAnswer(language);
            yield break;
        }

        var request = new OllamaGenerateRequest(
            Model: _options.OllamaModel,
            System: BuildSystemPrompt(language),
            Prompt: BuildUserPrompt(userText, intent, language, contextChunks),
            Stream: true,
            Options: new OllamaGenerateOptions(_options.OllamaTemperature));

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/generate")
        {
            Content = JsonContent.Create(request)
        };

        using var response = await _httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Ollama streaming failed with status {StatusCode}", response.StatusCode);
            yield return BuildFallbackAnswer(language);
            yield break;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(line))
                continue;

            OllamaGenerateResponse? chunk;

            try
            {
                chunk = JsonSerializer.Deserialize<OllamaGenerateResponse>(line);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Invalid Ollama stream chunk");
                continue;
            }

            if (!string.IsNullOrEmpty(chunk?.Response))
                yield return chunk.Response;

            if (chunk?.Done == true)
                yield break;
        }
    }

    private static string BuildSystemPrompt(string language)
    {
        return language switch
        {
            "ar" => """
أنت وكيل خدمة عملاء صوتي محترف.
استخدم فقط السياق المقدم.
لا تخترع أي معلومات.
اجعل الإجابة قصيرة ومناسبة للمحادثة الصوتية.
إذا لم تكن الإجابة موجودة في السياق، قل إنك لا تملك معلومات كافية واقترح التواصل مع موظف دعم.
أجب دائماً بالعربية.
""",
            "en" => """
You are a professional customer service voice assistant.
Use only the provided context.
Do not invent information.
Keep the answer short and voice-friendly.
If the answer is not in the context, say you do not have enough information and offer human handoff.
Always answer in English.
""",
            _ => """
Tu es un assistant vocal professionnel de service client.
Utilise uniquement le contexte fourni.
N’invente jamais d’informations.
Garde une réponse courte et adaptée à une conversation vocale.
Si la réponse n’est pas dans le contexte, dis que tu n’as pas assez d’informations et propose un transfert vers un conseiller humain.
Réponds toujours en français.
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

    private static string BuildNoContextAnswer(string language) => language switch
    {
        "ar" => "لا أملك معلومات كافية للإجابة على سؤالك بدقة. هل ترغب في التواصل مع موظف دعم؟",
        "en" => "I don’t have enough information to answer your question accurately. Would you like to be connected to a human agent?",
        _ => "Je n’ai pas suffisamment d’informations pour répondre précisément à votre question. Souhaitez-vous être mis en relation avec un conseiller ?"
    };

    private static string BuildFallbackAnswer(string language) => language switch
    {
        "ar" => "لا أستطيع معالجة الطلب حالياً. يمكنني تحويلك إلى موظف دعم.",
        "en" => "I cannot process the request right now. I can route you to a human agent.",
        _ => "Je ne peux pas traiter la demande pour le moment. Je peux vous transférer vers un conseiller humain."
    };
}