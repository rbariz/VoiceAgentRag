using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAgentRag.Contracts.Conversations
{
    public sealed record ConversationDto(
     Guid Id,
     string? CustomerReference,
     string Status,
     DateTime CreatedAtUtc);

}
