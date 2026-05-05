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



        [HttpPost("audio")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<VoiceAgentAudioResponse>> HandleAudio(
    IFormFile audioFile,
    [FromForm] string? language,
    [FromForm] string? customerReference,
    [FromForm] Guid? conversationId,
    CancellationToken cancellationToken)
        {
            if (audioFile is null || audioFile.Length == 0)
                return Problem(
                    title: "Invalid request",
                    detail: "Audio file is required.",
                    statusCode: StatusCodes.Status400BadRequest);

            await using var stream = audioFile.OpenReadStream();

            var response = await _voiceAgentService.HandleAudioAsync(
                stream,
                language,
                customerReference,
                conversationId,
                cancellationToken);

            return Ok(response);
        }


        [HttpPost("text/speak")]
        public async Task<ActionResult<VoiceAgentSpeakResponse>> HandleTextAndSpeak(
    VoiceAgentTextRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _voiceAgentService.HandleTextAndSpeakAsync(
                request,
                cancellationToken);

            return Ok(response);
        }
    }
}
