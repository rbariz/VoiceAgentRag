using VoiceAgentRag.Contracts.Voice;

namespace VoiceAgentRag.Application.Voice
{
    public interface IVoiceAgentStreamingService
    {
        IAsyncEnumerable<VoiceAgentStreamEvent> HandleTextStreamAsync(
    VoiceAgentTextRequest request,
    CancellationToken cancellationToken = default);
    }
}
