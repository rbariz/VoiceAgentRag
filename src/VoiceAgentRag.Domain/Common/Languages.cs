namespace VoiceAgentRag.Domain.Common
{
    public static class Languages
    {
        public const string French = "fr";
        public const string English = "en";
        public const string Arabic = "ar";

        public static readonly HashSet<string> Supported = new()
    {
        French,
        English,
        Arabic
    };

        public static bool IsSupported(string lang)
            => Supported.Contains(lang);
    }
}
