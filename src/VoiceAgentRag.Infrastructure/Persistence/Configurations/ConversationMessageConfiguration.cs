using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoiceAgentRag.Domain.Conversations;

namespace VoiceAgentRag.Infrastructure.Persistence.Configurations
{
    public sealed class ConversationMessageConfiguration : IEntityTypeConfiguration<ConversationMessage>
    {
        public void Configure(EntityTypeBuilder<ConversationMessage> builder)
        {
            builder.ToTable("conversation_messages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.ConversationId).HasColumnName("conversation_id");

            builder.Property(x => x.Role)
                .HasColumnName("role")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.Content)
                .HasColumnName("content")
                .IsRequired();

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
