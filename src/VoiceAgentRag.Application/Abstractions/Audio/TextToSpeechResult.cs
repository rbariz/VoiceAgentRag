namespace VoiceAgentRag.Application.Abstractions.Audio
{
    public sealed record TextToSpeechResult(
    byte[] AudioBytes,
    string ContentType,
    string FileExtension);
}
