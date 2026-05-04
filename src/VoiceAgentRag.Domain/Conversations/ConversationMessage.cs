using VoiceAgentRag.Domain.Common;

namespace VoiceAgentRag.Domain.Conversations
{
    public sealed class ConversationMessage : Entity
    {
        private ConversationMessage() { }

        public ConversationMessage(Guid conversationId, MessageRole role, string content)
        {
            if (conversationId == Guid.Empty)
                throw new ArgumentException("Conversation id is required.", nameof(conversationId));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Message content is required.", nameof(content));

            ConversationId = conversationId;
            Role = role;
            Content = content.Trim();
        }

        public Guid ConversationId { get; private set; }
        public MessageRole Role { get; private set; }
        public string Content { get; private set; } = string.Empty;
    }
}
