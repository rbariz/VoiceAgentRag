using VoiceAgentRag.Domain.Common;

namespace VoiceAgentRag.Domain.Conversations
{
    public sealed class Conversation : Entity
    {
        private readonly List<ConversationMessage> _messages = [];

        private Conversation() { }

        public Conversation(string? customerReference = null, string? language = null)
        {
            CustomerReference = customerReference;

            Language = Languages.IsSupported(language ?? "")
                ? language!
                : Languages.French;

            Status = ConversationStatus.Active;
        }

        public string? CustomerReference { get; private set; }
        public ConversationStatus Status { get; private set; }

        public string Language { get; private set; } = Languages.French;

        public IReadOnlyCollection<ConversationMessage> Messages => _messages.AsReadOnly();

        

        public void AddMessage(MessageRole role, string content, string? language = null)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Message content is required.", nameof(content));

            var lang = Languages.IsSupported(language ?? "")
                ? language!
                : Language;

            _messages.Add(new ConversationMessage(Id, role, content, lang));
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
