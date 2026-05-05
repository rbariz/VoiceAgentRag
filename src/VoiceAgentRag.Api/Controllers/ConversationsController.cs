using Microsoft.AspNetCore.Mvc;
using VoiceAgentRag.Application.Conversations;
using VoiceAgentRag.Contracts.Conversations;

namespace VoiceAgentRag.Api.Controllers
{
    [ApiController]
    [Route("api/conversations")]
    public sealed class ConversationsController : ControllerBase
    {
        private readonly IConversationQueryService _conversationQueryService;

        public ConversationsController(IConversationQueryService conversationQueryService)
        {
            _conversationQueryService = conversationQueryService;
        }

        [HttpGet("{conversationId:guid}/history")]
        public async Task<ActionResult<ConversationHistoryDto>> GetHistory(
            Guid conversationId,
            CancellationToken cancellationToken)
        {
            var history = await _conversationQueryService.GetHistoryAsync(
                conversationId,
                cancellationToken);

            if (history is null)
                return NotFound();

            return Ok(history);
        }
    }
}
