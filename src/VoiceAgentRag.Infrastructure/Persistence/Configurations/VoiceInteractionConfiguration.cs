using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoiceAgentRag.Domain.Voice;

namespace VoiceAgentRag.Infrastructure.Persistence.Configurations
{
    public sealed class VoiceInteractionConfiguration : IEntityTypeConfiguration<VoiceInteraction>
    {
        public void Configure(EntityTypeBuilder<VoiceInteraction> builder)
        {
            builder.ToTable("voice_interactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.ConversationId).HasColumnName("conversation_id");
            builder.Property(x => x.AudioInputPath).HasColumnName("audio_input_path").HasMaxLength(500);
            builder.Property(x => x.Transcription).HasColumnName("transcription").IsRequired();
            builder.Property(x => x.ResponseText).HasColumnName("response_text").IsRequired();
            builder.Property(x => x.AudioOutputPath).HasColumnName("audio_output_path").HasMaxLength(500);

            builder.Property(x => x.Language)
                .HasColumnName("language")
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            builder.HasIndex(x => x.ConversationId);
        }
    }
}
