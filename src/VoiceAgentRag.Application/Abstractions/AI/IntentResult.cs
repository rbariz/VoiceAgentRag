using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAgentRag.Application.Abstractions.AI
{
    public sealed record IntentResult(
    string Name,
    double Confidence,
    bool RequiresHumanHandoff);
}
