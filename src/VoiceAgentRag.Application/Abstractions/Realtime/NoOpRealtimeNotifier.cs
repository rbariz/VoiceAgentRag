namespace VoiceAgentRag.Application.Abstractions.Realtime
{
    public sealed class NoOpRealtimeNotifier : IRealtimeNotifier
    {
        public Task ConversationStartedAsync(
            Guid conversationId,
            string language,
            string? customerReference,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task UserMessageReceivedAsync(
            Guid conversationId,
            string language,
            string text,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task AssistantTokenGeneratedAsync(
            Guid conversationId,
            string token,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task AssistantResponseCompletedAsync(
            Guid conversationId,
            string language,
            string answer,
            string intent,
            bool requiresHumanHandoff,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task HumanHandoffRequestedAsync(
            Guid conversationId,
            string language,
            string intent,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
