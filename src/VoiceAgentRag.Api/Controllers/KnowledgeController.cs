using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoiceAgentRag.Application.Knowledge;
using VoiceAgentRag.Contracts.Knowledge;

namespace VoiceAgentRag.Api.Controllers
{
    [ApiController]
    [Route("api/knowledge")]
    public sealed class KnowledgeController : ControllerBase
    {
        private readonly IKnowledgeIngestionService _ingestionService;

        public KnowledgeController(IKnowledgeIngestionService ingestionService)
        {
            _ingestionService = ingestionService;
        }

        [HttpPost("ingest")]
        public async Task<ActionResult<IngestDocumentResponse>> Ingest(
            IngestDocumentRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _ingestionService.IngestAsync(request, cancellationToken);
            return Ok(response);
        }
    }
}
