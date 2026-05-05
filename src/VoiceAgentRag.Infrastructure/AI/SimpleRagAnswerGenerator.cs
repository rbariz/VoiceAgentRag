using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Application.Abstractions.AI;

namespace VoiceAgentRag.Infrastructure.AI
{

    public sealed class SimpleRagAnswerGenerator : IAnswerGenerator
    {
        public Task<string> GenerateAnswerAsync(
            string userText,
            string intent,
            string language,
            IReadOnlyList<string> contextChunks,
            CancellationToken cancellationToken = default)
        {
            if (contextChunks.Count == 0)
            {
                return Task.FromResult(language switch
                {
                    "ar" => "لا أملك معلومات كافية للإجابة بدقة. يمكنني تحويل طلبك إلى موظف دعم.",
                    "en" => "I do not have enough information to answer accurately. I can route this request to a human agent.",
                    _ => "Je n’ai pas assez d’informations pour répondre avec précision. Je peux transférer votre demande à un conseiller humain."
                });
            }

            var context = string.Join("\n\n", contextChunks.Take(3));

            var answer = language switch
            {
                "ar" =>
                    $"حسب المعلومات المتوفرة في قاعدة المعرفة:\n\n{context}\n\nإذا كنت تحتاج إلى تفاصيل إضافية، يمكنني تحويل طلبك إلى موظف دعم.",

                "en" =>
                    $"According to the available knowledge base:\n\n{context}\n\nIf you need more details, I can route your request to a human agent.",

                _ =>
                    $"D’après les informations disponibles dans la base de connaissance :\n\n{context}\n\nSi vous avez besoin de plus de détails, je peux transférer votre demande à un conseiller humain."
            };

            return Task.FromResult(answer);
        }
    }
}
