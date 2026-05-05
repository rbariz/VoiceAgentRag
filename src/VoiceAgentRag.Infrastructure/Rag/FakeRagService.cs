using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Application.Abstractions.Rag;

namespace VoiceAgentRag.Infrastructure.Rag
{
    public sealed class FakeRagService : IRagService
    {
        public Task<IReadOnlyList<string>> RetrieveContextAsync(
            string query,
            string language,
            int maxChunks = 5,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<string> result = [];
            return Task.FromResult(result);
        }
    }
}
