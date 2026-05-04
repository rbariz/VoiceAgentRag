using VoiceAgentRag.Domain.Common;

namespace VoiceAgentRag.Domain.Conversations
{
    public sealed class Conversation : Entity
    {
        private readonly List<ConversationMessage> _messages = [];

        private Conversation() { }

        public Conversation(string? customerReference = null)
        {
            CustomerReference = customerReference;
            Status = ConversationStatus.Active;
        }

        public string? CustomerReference { get; private set; }
        public ConversationStatus Status { get; private set; }

        public IReadOnlyCollection<ConversationMessage> Messages => _messages.AsReadOnly();

        public void AddMessage(MessageRole role, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Message content is required.", nameof(content));

            _messages.Add(new ConversationMessage(Id, role, content));
        }

        public void Complete()
        {
            Status = ConversationStatus.Completed;
        }

        public void EscalateToHuman()
        {
            Status = ConversationStatus.EscalatedToHuman;
        }
    }
}
