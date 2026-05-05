using VoiceAgentRag.Domain.Common;

namespace VoiceAgentRag.Domain.Conversations
{
    public sealed class ConversationMessage : Entity
    {
        private ConversationMessage() { }

        public ConversationMessage(Guid conversationId, MessageRole role, string content, string? language = null)
        {
            if (conversationId == Guid.Empty)
                throw new ArgumentException("Conversation id is required.");

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Message content is required.");

            ConversationId = conversationId;
            Role = role;
            Content = content.Trim();

            Language = Languages.IsSupported(language ?? "")
                ? language!
                : Languages.French;
        }

        public Guid ConversationId { get; private set; }
        public MessageRole Role { get; private set; }
        public string Content { get; private set; } = string.Empty;

        public string Language { get; private set; } = Languages.French;
    }
}
