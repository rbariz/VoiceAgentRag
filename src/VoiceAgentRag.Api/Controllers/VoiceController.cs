using Microsoft.AspNetCore.Mvc;
using VoiceAgentRag.Application.Voice;
using VoiceAgentRag.Contracts.Voice;

namespace VoiceAgentRag.Api.Controllers
{
    [ApiController]
    [Route("api/voice-agent")]
    public sealed class VoiceController : ControllerBase
    {
        private readonly IVoiceAgentService _voiceAgentService;

        public VoiceController(IVoiceAgentService voiceAgentService)
        {
            _voiceAgentService = voiceAgentService;
        }

        [HttpPost("text")]
        public async Task<ActionResult<VoiceAgentResponse>> HandleText(
            VoiceAgentTextRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _voiceAgentService.HandleTextAsync(request, cancellationToken);
            return Ok(response);
        }
    }
}
