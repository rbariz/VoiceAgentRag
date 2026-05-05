using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoiceAgentRag.Domain.Knowledge;

namespace VoiceAgentRag.Infrastructure.Persistence.Configurations
{
    public sealed class KnowledgeDocumentConfiguration : IEntityTypeConfiguration<KnowledgeDocument>
    {
        public void Configure(EntityTypeBuilder<KnowledgeDocument> builder)
        {
            builder.ToTable("knowledge_documents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(250).IsRequired();
            builder.Property(x => x.Source).HasColumnName("source").HasMaxLength(500).IsRequired();
            builder.Property(x => x.Content).HasColumnName("content").IsRequired();

            builder.Property(x => x.Language)
                .HasColumnName("language")
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            builder.HasIndex(x => x.Language);
            builder.HasIndex(x => x.Source);
        }
    }
}
