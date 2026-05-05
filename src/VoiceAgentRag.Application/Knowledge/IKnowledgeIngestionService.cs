using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Contracts.Knowledge;

namespace VoiceAgentRag.Application.Knowledge
{

    public interface IKnowledgeIngestionService
    {
        Task<IngestDocumentResponse> IngestAsync(
            IngestDocumentRequest request,
            CancellationToken cancellationToken = default);
    }
}
