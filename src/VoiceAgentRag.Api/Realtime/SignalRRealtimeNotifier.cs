using Microsoft.AspNetCore.SignalR;
using VoiceAgentRag.Application.Abstractions.Realtime;

namespace VoiceAgentRag.Api.Realtime
{
    public sealed class SignalRRealtimeNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<VoiceAgentHub> _hub;

        public SignalRRealtimeNotifier(IHubContext<VoiceAgentHub> hub)
        {
            _hub = hub;
        }

        public Task ConversationStartedAsync(
            Guid conversationId,
            string language,
            string? customerReference,
            CancellationToken cancellationToken = default)
        {
            return _hub.Clients.Group("ops").SendAsync(
                "ConversationStarted",
                new
                {
                    conversationId,
                    language,
                    customerReference,
                    startedAtUtc = DateTime.UtcNow
                },
                cancellationToken);
        }

        public Task UserMessageReceivedAsync(
            Guid conversationId,
            string language,
            string text,
            CancellationToken cancellationToken = default)
        {
            return _hub.Clients.Group("ops").SendAsync(
                "UserMessageReceived",
                new
                {
                    conversationId,
                    language,
                    text,
                    receivedAtUtc = DateTime.UtcNow
                },
                cancellationToken);
        }

        public Task AssistantTokenGeneratedAsync(
            Guid conversationId,
            string token,
            CancellationToken cancellationToken = default)
        {
            return _hub.Clients.Group("ops").SendAsync(
                "AssistantTokenGenerated",
                new
                {
                    conversationId,
                    token
                },
                cancellationToken);
        }

        public Task AssistantResponseCompletedAsync(
            Guid conversationId,
            string language,
            string answer,
            string intent,
            bool requiresHumanHandoff,
            CancellationToken cancellationToken = default)
        {
            return _hub.Clients.Group("ops").SendAsync(
                "AssistantResponseCompleted",
                new
                {
                    conversationId,
                    language,
                    answer,
                    intent,
                    requiresHumanHandoff,
                    completedAtUtc = DateTime.UtcNow
                },
                cancellationToken);
        }

        public Task HumanHandoffRequestedAsync(
            Guid conversationId,
            string language,
            string intent,
            CancellationToken cancellationToken = default)
        {
            return _hub.Clients.Group("ops").SendAsync(
                "HumanHandoffRequested",
                new
                {
                    conversationId,
                    language,
                    intent,
                    requestedAtUtc = DateTime.UtcNow
                },
                cancellationToken);
        }
    }
}
