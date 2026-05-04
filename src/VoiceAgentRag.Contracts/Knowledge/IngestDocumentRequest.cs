using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAgentRag.Contracts.Knowledge
{
    public sealed record IngestDocumentRequest(
    string Title,
    string Source,
    string Content);
}
