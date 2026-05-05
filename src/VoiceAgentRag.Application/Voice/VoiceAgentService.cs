using VoiceAgentRag.Application.Abstractions.AI;
using VoiceAgentRag.Application.Abstractions.Audio;
using VoiceAgentRag.Application.Abstractions.Persistence;
using VoiceAgentRag.Application.Abstractions.Rag;
using VoiceAgentRag.Application.Common;
using VoiceAgentRag.Contracts.Voice;
using VoiceAgentRag.Domain.Common;
using VoiceAgentRag.Domain.Conversations;
using VoiceAgentRag.Domain.Voice;

namespace VoiceAgentRag.Application.Voice
{
    public sealed class VoiceAgentService : IVoiceAgentService
    {
        private readonly IConversationRepository _conversations;
        private readonly IRagService _rag;
        private readonly IIntentDetector _intentDetector;
        private readonly IAnswerGenerator _answerGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISpeechToTextService _speechToText;
        private readonly ITextToSpeechService _textToSpeech;
        private readonly IVoiceInteractionRepository _voiceInteractions;
        public VoiceAgentService(
            IConversationRepository conversations,
            IRagService rag,
            IIntentDetector intentDetector,
            IAnswerGenerator answerGenerator,
            IUnitOfWork unitOfWork,
            ISpeechToTextService speechToText,
            ITextToSpeechService textToSpeech,
            IVoiceInteractionRepository voiceInteractions)
        {
            _conversations = conversations;
            _rag = rag;
            _intentDetector = intentDetector;
            _answerGenerator = answerGenerator;
            _unitOfWork = unitOfWork;
            _speechToText = speechToText;
            _textToSpeech = textToSpeech;
            _voiceInteractions = voiceInteractions;
        }

        public async Task<VoiceAgentResponse> HandleTextAsync(
            VoiceAgentTextRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.UserText))
                throw new ArgumentException("User text is required.");

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
                        $"Conversation language is '{conversation.Language}', but request language is '{language}'. Start a new conversation to switch language.");
                }
            }
            else
            {
                conversation = new Conversation(request.CustomerReference, language);
                _conversations.Add(conversation);
            }

            //conversation.AddMessage(MessageRole.User, request.UserText, language);

            _conversations.AddMessage(new ConversationMessage(
                    conversation.Id,
                    MessageRole.User,
                    request.UserText,
                    language));

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

            //conversation.AddMessage(MessageRole.Assistant, answer, language);
            _conversations.AddMessage(new ConversationMessage(
                    conversation.Id,
                    MessageRole.Assistant,
                    answer,
                    language));

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



        public async Task<VoiceAgentAudioResponse> HandleAudioAsync(
    Stream audioStream,
    string? language,
    string? customerReference,
    Guid? conversationId,
    CancellationToken cancellationToken = default)
        {
            var transcription = await _speechToText.TranscribeAsync(
                audioStream,
                language,
                cancellationToken);

            var textResponse = await HandleTextAsync(
                new VoiceAgentTextRequest(
                    conversationId,
                    transcription.Text,
                    transcription.Language,
                    customerReference),
                cancellationToken);

            _voiceInteractions.Add(new VoiceInteraction(
    textResponse.ConversationId,
    audioInputPath: null,
    transcription.Text,
    textResponse.Answer,
    audioOutputPath: null,
    textResponse.Language));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new VoiceAgentAudioResponse(
                textResponse.ConversationId,
                textResponse.Language,
                transcription.Text,
                textResponse.Intent,
                textResponse.Answer,
                textResponse.RequiresHumanHandoff);
        }


        public async Task<VoiceAgentSpeakResponse> HandleTextAndSpeakAsync(
    VoiceAgentTextRequest request,
    CancellationToken cancellationToken = default)
        {
            var textResponse = await HandleTextAsync(request, cancellationToken);


            var audio = await _textToSpeech.SynthesizeAsync(
                textResponse.Answer,
                textResponse.Language,
                cancellationToken);

            return new VoiceAgentSpeakResponse(
                textResponse.ConversationId,
                textResponse.Language,
                textResponse.Transcription,
                textResponse.Intent,
                textResponse.Answer,
                textResponse.RequiresHumanHandoff,
                audio.ContentType,
                Convert.ToBase64String(audio.AudioBytes));
        }
    }
}
