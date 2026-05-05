using VoiceAgentRag.Application.Abstractions.AI;

namespace VoiceAgentRag.Infrastructure.AI
{
    public sealed class FakeAnswerGenerator : IAnswerGenerator
    {
        public Task<string> GenerateAnswerAsync(
            string userText,
            string intent,
            string language,
            IReadOnlyList<string> contextChunks,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(language switch
            {
                "ar" => "تم استلام طلبك. هذه إجابة تجريبية مؤقتة.",
                "en" => "Your request has been received. This is a temporary test answer.",
                _ => "Votre demande a bien été reçue. Ceci est une réponse temporaire de test."
            });
        }
    }
}
