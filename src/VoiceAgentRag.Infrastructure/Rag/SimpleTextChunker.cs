using VoiceAgentRag.Application.Abstractions.Rag;

namespace VoiceAgentRag.Infrastructure.Rag
{
    public sealed class SimpleTextChunker : ITextChunker
    {
        public IReadOnlyList<string> Split(string text, int maxChunkLength = 1200)
        {
            if (string.IsNullOrWhiteSpace(text))
                return [];

            var normalized = text.Replace("\r\n", "\n").Trim();

            var paragraphs = normalized
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var chunks = new List<string>();
            var current = string.Empty;

            foreach (var paragraph in paragraphs)
            {
                if (paragraph.Length > maxChunkLength)
                {
                    Flush();

                    foreach (var part in SplitLongText(paragraph, maxChunkLength))
                        chunks.Add(part);

                    continue;
                }

                if (current.Length + paragraph.Length + 2 > maxChunkLength)
                    Flush();

                current = string.IsNullOrWhiteSpace(current)
                    ? paragraph
                    : $"{current}\n\n{paragraph}";
            }

            Flush();

            return chunks;

            void Flush()
            {
                if (!string.IsNullOrWhiteSpace(current))
                {
                    chunks.Add(current.Trim());
                    current = string.Empty;
                }
            }
        }

        private static IEnumerable<string> SplitLongText(string text, int maxLength)
        {
            for (var i = 0; i < text.Length; i += maxLength)
            {
                var length = Math.Min(maxLength, text.Length - i);
                yield return text.Substring(i, length).Trim();
            }
        }
    }
}
