using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Domain.Conversations;

namespace VoiceAgentRag.Infrastructure.Persistence.Configurations
{
    public sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.ToTable("conversations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.CustomerReference).HasColumnName("customer_reference").HasMaxLength(120);
            builder.Property(x => x.Language).HasColumnName("language").HasMaxLength(5).IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            builder.HasMany(x => x.Messages)
                .WithOne()
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(x => x.Messages)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
