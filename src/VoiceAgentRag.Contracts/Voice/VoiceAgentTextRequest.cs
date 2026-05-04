using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAgentRag.Contracts.Voice
{
    public sealed record VoiceAgentTextRequest(
    Guid? ConversationId,
    string UserText,
     string? Language = "fr",
    string? CustomerReference = null);
}
