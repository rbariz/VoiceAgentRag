using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;
using VoiceAgentRag.Domain.Knowledge;

namespace VoiceAgentRag.Infrastructure.Persistence.Configurations
{
    public sealed class KnowledgeChunkConfiguration : IEntityTypeConfiguration<KnowledgeChunk>
    {
        public void Configure(EntityTypeBuilder<KnowledgeChunk> builder)
        {
            builder.ToTable("knowledge_chunks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.DocumentId).HasColumnName("document_id");
            builder.Property(x => x.Index).HasColumnName("chunk_index");
            builder.Property(x => x.Content).HasColumnName("content").IsRequired();

            builder.Property(x => x.Language)
                .HasColumnName("language")
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            builder.Ignore(x => x.Embedding);

            builder.HasIndex(x => x.DocumentId);
            builder.HasIndex(x => x.Language);

            builder.HasOne<KnowledgeDocument>()
                .WithMany()
                .HasForeignKey(x => x.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
