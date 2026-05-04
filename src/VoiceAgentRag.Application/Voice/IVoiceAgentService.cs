using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Contracts.Voice;

namespace VoiceAgentRag.Application.Voice
{
    public interface IVoiceAgentService
    {
        Task<VoiceAgentResponse> HandleTextAsync(
            VoiceAgentTextRequest request,
            CancellationToken cancellationToken = default);
    }
}
