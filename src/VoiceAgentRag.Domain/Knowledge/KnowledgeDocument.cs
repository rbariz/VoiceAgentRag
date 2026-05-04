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

        public KnowledgeDocument(string title, string source, string content)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Document title is required.", nameof(title));

            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("Document source is required.", nameof(source));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Document content is required.", nameof(content));

            Title = title.Trim();
            Source = source.Trim();
            Content = content.Trim();
        }

        public string Title { get; private set; } = string.Empty;
        public string Source { get; private set; } = string.Empty;
        public string Content { get; private set; } = string.Empty;
    }
}
