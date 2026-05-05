using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Domain.Common;

namespace VoiceAgentRag.Domain.Knowledge
{
    public sealed class KnowledgeDocument : Entity
    {
        private KnowledgeDocument() { }

        public KnowledgeDocument(string title, string source, string content, string? language = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Document title is required.");

            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("Document source is required.");

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Document content is required.");

            Title = title.Trim();
            Source = source.Trim();
            Content = content.Trim();

            Language = Languages.IsSupported(language ?? "")
                ? language!
                : Languages.French;
        }

        public string Title { get; private set; } = string.Empty;
        public string Source { get; private set; } = string.Empty;

        public string Language { get; private set; } = Languages.French;
        public string Content { get; private set; } = string.Empty;
    }
}
