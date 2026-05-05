using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAgentRag.Application.Abstractions.Audio;
using VoiceAgentRag.Domain.Common;

namespace VoiceAgentRag.Infrastructure.Audio
{
    public sealed class FakeSpeechToTextService : ISpeechToTextService
    {
        public Task<SpeechToTextResult> TranscribeAsync(
            Stream audioStream,
            string? language,
            CancellationToken cancellationToken = default)
        {
            var text = language switch
            {
                "ar" => "أريد تتبع طلبي",
                "en" => "I want to track my order",
                _ => "Je veux suivre ma commande"
            };

            return Task.FromResult(new SpeechToTextResult(
                text,
                language ?? "fr",
                null));
        }
    }
}
