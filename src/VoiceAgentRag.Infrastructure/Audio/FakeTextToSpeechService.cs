using System.Text;
using VoiceAgentRag.Application.Abstractions.Audio;

namespace VoiceAgentRag.Infrastructure.Audio
{
    public sealed class FakeTextToSpeechService : ITextToSpeechService
    {
        public Task<TextToSpeechResult> SynthesizeAsync(
            string text,
            string language,
            CancellationToken cancellationToken = default)
        {
            var fakeAudioContent = $"FAKE_AUDIO_OUTPUT::{language}::{text}";
            var bytes = Encoding.UTF8.GetBytes(fakeAudioContent);

            return Task.FromResult(new TextToSpeechResult(
                bytes,
                ContentType: "audio/wav",
                FileExtension: ".wav"));
        }
    }
}
