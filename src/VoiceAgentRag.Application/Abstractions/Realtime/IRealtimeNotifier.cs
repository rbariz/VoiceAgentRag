using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAgentRag.Application.Abstractions.Realtime
{
    public interface IRealtimeNotifier
    {
        Task ConversationStartedAsync(
            Guid conversationId,
            string language,
            string? customerReference,
            CancellationToken cancellationToken = default);

        Task UserMessageReceivedAsync(
            Guid conversationId,
            string language,
            string text,
            CancellationToken cancellationToken = default);

        Task AssistantTokenGeneratedAsync(
            Guid conversationId,
            string token,
            CancellationToken cancellationToken = default);

        Task AssistantResponseCompletedAsync(
            Guid conversationId,
            string language,
            string answer,
            string intent,
            bool requiresHumanHandoff,
            CancellationToken cancellationToken = default);

        Task HumanHandoffRequestedAsync(
            Guid conversationId,
            string language,
            string intent,
            CancellationToken cancellationToken = default);
    }
}
