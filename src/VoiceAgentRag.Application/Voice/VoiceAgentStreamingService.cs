using System.Runtime.CompilerServices;
using VoiceAgentRag.Application.Abstractions.AI;
using VoiceAgentRag.Application.Abstractions.Persistence;
using VoiceAgentRag.Application.Abstractions.Rag;
using VoiceAgentRag.Application.Common;
using VoiceAgentRag.Contracts.Voice;
using VoiceAgentRag.Domain.Common;
using VoiceAgentRag.Domain.Conversations;

namespace VoiceAgentRag.Application.Voice
{
    public sealed class VoiceAgentStreamingService : IVoiceAgentStreamingService
    {
        private readonly IConversationRepository _conversations;
        private readonly IRagService _rag;
        private readonly IIntentDetector _intentDetector;
        private readonly IStreamingAnswerGenerator _streamingAnswerGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public VoiceAgentStreamingService(
            IConversationRepository conversations,
            IRagService rag,
            IIntentDetector intentDetector,
            IStreamingAnswerGenerator streamingAnswerGenerator,
            IUnitOfWork unitOfWork)
        {
            _conversations = conversations;
            _rag = rag;
            _intentDetector = intentDetector;
            _streamingAnswerGenerator = streamingAnswerGenerator;
            _unitOfWork = unitOfWork;
        }

        public async IAsyncEnumerable<VoiceAgentStreamEvent> HandleTextStreamAsync(
     VoiceAgentTextRequest request,
     [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.UserText))
                throw new ValidationException("User text is required.");

            var language = Languages.IsSupported(request.Language ?? "")
                ? request.Language!
                : Languages.French;

            Conversation conversation;

            if (request.ConversationId.HasValue)
            {
                conversation = await _conversations.GetByIdAsync(
                    request.ConversationId.Value,
                    cancellationToken)
                    ?? throw new InvalidOperationException("Conversation not found.");

                if (conversation.Language != language)
                {
                    throw new ValidationException(
                        $"Conversation language is '{conversation.Language}', but request language is '{language}'.");
                }
            }
            else
            {
                conversation = new Conversation(request.CustomerReference, language);
                _conversations.Add(conversation);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            _conversations.AddMessage(new ConversationMessage(
                conversation.Id,
                MessageRole.User,
                request.UserText,
                language));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var intent = await _intentDetector.DetectAsync(
                request.UserText,
                language,
                cancellationToken);

            var contextChunks = await _rag.RetrieveContextAsync(
                request.UserText,
                language,
                maxChunks: 5,
                cancellationToken);

            yield return new VoiceAgentStreamEvent(
                Type: "metadata",
                ConversationId: conversation.Id,
                Language: language,
                Intent: intent.Name,
                RequiresHumanHandoff: intent.RequiresHumanHandoff,
                Token: null);

            var fullAnswer = "";

            await foreach (var token in _streamingAnswerGenerator.GenerateAnswerStreamAsync(
                request.UserText,
                intent.Name,
                language,
                contextChunks,
                cancellationToken))
            {
                fullAnswer += token;

                yield return new VoiceAgentStreamEvent(
                    Type: "token",
                    ConversationId: null,
                    Language: null,
                    Intent: null,
                    RequiresHumanHandoff: null,
                    Token: token);
            }

            _conversations.AddMessage(new ConversationMessage(
                conversation.Id,
                MessageRole.Assistant,
                fullAnswer,
                language));

            if (intent.RequiresHumanHandoff)
                conversation.EscalateToHuman();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            yield return new VoiceAgentStreamEvent(
                Type: "done",
                ConversationId: conversation.Id,
                Language: language,
                Intent: intent.Name,
                RequiresHumanHandoff: intent.RequiresHumanHandoff,
                Token: null);
        }
    }
}
