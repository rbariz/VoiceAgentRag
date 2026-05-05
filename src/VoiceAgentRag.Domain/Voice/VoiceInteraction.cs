using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Domain.Common;

namespace VoiceAgentRag.Domain.Voice
{

    public sealed class VoiceInteraction : Entity
    {
        private VoiceInteraction() { }

        public VoiceInteraction(
         Guid conversationId,
         string? audioInputPath,
         string transcription,
         string responseText,
         string? audioOutputPath,
         string? language = null)
        {
            if (conversationId == Guid.Empty)
                throw new ArgumentException("Conversation id is required.");

            if (string.IsNullOrWhiteSpace(transcription))
                throw new ArgumentException("Transcription is required.");

            if (string.IsNullOrWhiteSpace(responseText))
                throw new ArgumentException("Response text is required.");

            ConversationId = conversationId;
            AudioInputPath = audioInputPath;
            Transcription = transcription.Trim();
            ResponseText = responseText.Trim();
            AudioOutputPath = audioOutputPath;

            Language = Languages.IsSupported(language ?? "")
                ? language!
                : Languages.French;
        }

        public Guid ConversationId { get; private set; }
        public string? AudioInputPath { get; private set; }
        public string Transcription { get; private set; } = string.Empty;
        public string ResponseText { get; private set; } = string.Empty;
        public string? AudioOutputPath { get; private set; }

        public string Language { get; private set; } = Languages.French;
    }
}
