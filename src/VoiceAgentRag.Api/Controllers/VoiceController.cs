using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VoiceAgentRag.Application.Voice;
using VoiceAgentRag.Contracts.Voice;

namespace VoiceAgentRag.Api.Controllers
{
    [ApiController]
    [Route("api/voice-agent")]
    public sealed class VoiceController : ControllerBase
    {
        private readonly IVoiceAgentService _voiceAgentService;
        private readonly IVoiceAgentStreamingService _streamingService;

        public VoiceController(IVoiceAgentService voiceAgentService, IVoiceAgentStreamingService streamingService)
        {
            _voiceAgentService = voiceAgentService;
            _streamingService = streamingService;
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

        //    [HttpPost("text/stream")]
        //    public async Task StreamText(
        //VoiceAgentTextRequest request,
        //CancellationToken cancellationToken)
        //    {
        //        Response.ContentType = "application/x-ndjson; charset=utf-8";

        //        await foreach (var item in _streamingService.HandleTextStreamAsync(request, cancellationToken))
        //        {
        //            await Response.WriteAsJsonAsync(item, cancellationToken);
        //            await Response.WriteAsync("\n", cancellationToken);
        //            await Response.Body.FlushAsync(cancellationToken);
        //        }
        //    }

        [HttpPost("text/stream")]
        public async Task StreamText(
    VoiceAgentTextRequest request,
    CancellationToken cancellationToken)
        {
            Response.ContentType = "application/x-ndjson; charset=utf-8";

            await foreach (var item in _streamingService.HandleTextStreamAsync(request, cancellationToken))
            {
                var json = JsonSerializer.Serialize(item);
                await Response.WriteAsync(json + "\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
    }
}
