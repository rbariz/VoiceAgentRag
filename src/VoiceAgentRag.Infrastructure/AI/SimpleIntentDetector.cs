using VoiceAgentRag.Application.Abstractions.AI;

namespace VoiceAgentRag.Infrastructure.AI
{
    public sealed class SimpleIntentDetector : IIntentDetector
    {
        public Task<IntentResult> DetectAsync(
            string userText,
            string language,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userText))
            {
                return Task.FromResult(new IntentResult(
                    "unknown",
                    0.0,
                    RequiresHumanHandoff: true));
            }

            var text = userText.Trim().ToLowerInvariant();

            var result = language switch
            {
                "en" => DetectEnglish(text),
                "ar" => DetectArabic(text),
                _ => DetectFrench(text)
            };

            return Task.FromResult(result);
        }

        private static IntentResult DetectFrench(string text)
        {
            if (ContainsAny(text, "livraison", "commande", "expédition", "suivi", "colis"))
                return new IntentResult("order_or_delivery", 0.85, false);

            if (ContainsAny(text, "rendez-vous", "rdv", "réserver", "planifier", "horaire"))
                return new IntentResult("appointment", 0.85, false);

            if (ContainsAny(text, "remboursement", "annulation", "retour", "réclamation"))
                return new IntentResult("refund_or_complaint", 0.8, true);

            if (ContainsAny(text, "conseiller", "agent", "humain", "responsable"))
                return new IntentResult("human_handoff", 0.95, true);

            return new IntentResult("general_question", 0.55, false);
        }

        private static IntentResult DetectEnglish(string text)
        {
            if (ContainsAny(text, "delivery", "order", "shipping", "tracking", "package"))
                return new IntentResult("order_or_delivery", 0.85, false);

            if (ContainsAny(text, "appointment", "book", "schedule", "time slot"))
                return new IntentResult("appointment", 0.85, false);

            if (ContainsAny(text, "refund", "cancel", "return", "complaint"))
                return new IntentResult("refund_or_complaint", 0.8, true);

            if (ContainsAny(text, "agent", "human", "advisor", "manager"))
                return new IntentResult("human_handoff", 0.95, true);

            return new IntentResult("general_question", 0.55, false);
        }

        private static IntentResult DetectArabic(string text)
        {
            if (ContainsAny(text, "توصيل", "طلب", "شحن", "تتبع", "طرد"))
                return new IntentResult("order_or_delivery", 0.85, false);

            if (ContainsAny(text, "موعد", "حجز", "جدولة", "وقت"))
                return new IntentResult("appointment", 0.85, false);

            if (ContainsAny(text, "استرجاع", "إلغاء", "إرجاع", "شكوى"))
                return new IntentResult("refund_or_complaint", 0.8, true);

            if (ContainsAny(text, "موظف", "إنسان", "مسؤول", "وكيل"))
                return new IntentResult("human_handoff", 0.95, true);

            return new IntentResult("general_question", 0.55, false);
        }

        private static bool ContainsAny(string text, params string[] terms)
        {
            return terms.Any(text.Contains);
        }
    }
}
