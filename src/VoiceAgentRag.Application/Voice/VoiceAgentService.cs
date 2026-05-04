using VoiceAgentRag.Application.Abstractions.AI;
using VoiceAgentRag.Application.Abstractions.Persistence;
using VoiceAgentRag.Application.Abstractions.Rag;
using VoiceAgentRag.Contracts.Voice;
using VoiceAgentRag.Domain.Common;
using VoiceAgentRag.Domain.Conversations;

namespace VoiceAgentRag.Application.Voice
{
    public sealed class VoiceAgentService : IVoiceAgentService
    {
        private readonly IConversationRepository _conversations;
        private readonly IRagService _rag;
        private readonly IIntentDetector _intentDetector;
        private readonly IAnswerGenerator _answerGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public VoiceAgentService(
            IConversationRepository conversations,
            IRagService rag,
            IIntentDetector intentDetector,
            IAnswerGenerator answerGenerator,
            IUnitOfWork unitOfWork)
        {
            _conversations = conversations;
            _rag = rag;
            _intentDetector = intentDetector;
            _answerGenerator = answerGenerator;
            _unitOfWork = unitOfWork;
        }

        public async Task<VoiceAgentResponse> HandleTextAsync(
            VoiceAgentTextRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.UserText))
                throw new ArgumentException("User text is required.", nameof(request));

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
            }
            else
            {
                conversation = new Conversation(request.CustomerReference, language);
                _conversations.Add(conversation);
            }

            conversation.AddMessage(MessageRole.User, request.UserText, language);

            var intent = await _intentDetector.DetectAsync(
                request.UserText,
                language,
                cancellationToken);

            var contextChunks = await _rag.RetrieveContextAsync(
                request.UserText,
                language,
                maxChunks: 5,
                cancellationToken);

            var answer = await _answerGenerator.GenerateAnswerAsync(
                request.UserText,
                intent.Name,
                language,
                contextChunks,
                cancellationToken);

            conversation.AddMessage(MessageRole.Assistant, answer, language);

            if (intent.RequiresHumanHandoff)
                conversation.EscalateToHuman();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new VoiceAgentResponse(
                conversation.Id,
                language,
                request.UserText,
                intent.Name,
                answer,
                intent.RequiresHumanHandoff);
        }
    }
}
