using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAgentRag.Application.Abstractions.Audio
{
    public sealed record SpeechToTextResult(
    string Text,
    string Language,
    double? Confidence = null);
}
