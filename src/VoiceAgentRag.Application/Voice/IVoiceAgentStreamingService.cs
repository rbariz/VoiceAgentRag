using VoiceAgentRag.Contracts.Voice;

namespace VoiceAgentRag.Application.Voice
{
    public interface IVoiceAgentStreamingService
    {
        IAsyncEnumerable<VoiceAgentStreamEvent> HandleTextStreamAsync(
    VoiceAgentTextRequest request,
    CancellationToken cancellationToken = default);

        IAsyncEnumerable<VoiceAgentStreamEvent> HandleAudioStreamAsync(
    Stream audioStream,
    string? language,
    string? customerReference,
    Guid? conversationId,
    CancellationToken cancellationToken = default);
    }
}
