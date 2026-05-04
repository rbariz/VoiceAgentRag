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
            string? audioOutputPath)
        {
            if (conversationId == Guid.Empty)
                throw new ArgumentException("Conversation id is required.", nameof(conversationId));

            if (string.IsNullOrWhiteSpace(transcription))
                throw new ArgumentException("Transcription is required.", nameof(transcription));

            if (string.IsNullOrWhiteSpace(responseText))
                throw new ArgumentException("Response text is required.", nameof(responseText));

            ConversationId = conversationId;
            AudioInputPath = audioInputPath;
            Transcription = transcription.Trim();
            ResponseText = responseText.Trim();
            AudioOutputPath = audioOutputPath;
        }

        public Guid ConversationId { get; private set; }
        public string? AudioInputPath { get; private set; }
        public string Transcription { get; private set; } = string.Empty;
        public string ResponseText { get; private set; } = string.Empty;
        public string? AudioOutputPath { get; private set; }

        public string Language { get; private set; } = Languages.French;
    }
}
