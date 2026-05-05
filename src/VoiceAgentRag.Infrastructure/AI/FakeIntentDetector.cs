using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Application.Abstractions.AI;

namespace VoiceAgentRag.Infrastructure.AI
{
    public sealed class FakeIntentDetector : IIntentDetector
    {
        public Task<IntentResult> DetectAsync(
            string userText,
            string language,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new IntentResult(
                Name: "general_question",
                Confidence: 0.8,
                RequiresHumanHandoff: false));
        }
    }
}
