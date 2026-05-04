using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAgentRag.Application.Abstractions.Rag
{
    public interface IRagService
    {
        Task<IReadOnlyList<string>> RetrieveContextAsync(
            string query,
            string language,
            int maxChunks = 5,
            CancellationToken cancellationToken = default);
    }


}
