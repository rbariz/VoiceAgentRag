window.tts = {
    speak: function (text, lang) {
        if (!window.speechSynthesis) return;

        const utterance = new SpeechSynthesisUtterance(text);

        utterance.lang = lang === "ar"
            ? "ar-SA"
            : lang === "en"
                ? "en-US"
                : "fr-FR";

        utterance.rate = 1;
        utterance.pitch = 1;

        speechSynthesis.cancel(); // stop previous
        speechSynthesis.speak(utterance);
    }
};