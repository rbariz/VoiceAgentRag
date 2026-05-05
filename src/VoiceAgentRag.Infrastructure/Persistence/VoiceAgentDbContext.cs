using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Domain.Conversations;
using VoiceAgentRag.Domain.Knowledge;
using VoiceAgentRag.Domain.Voice;

namespace VoiceAgentRag.Infrastructure.Persistence
{
    public sealed class VoiceAgentDbContext : DbContext
    {
        public VoiceAgentDbContext(DbContextOptions<VoiceAgentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<ConversationMessage> ConversationMessages => Set<ConversationMessage>();
        public DbSet<KnowledgeDocument> KnowledgeDocuments => Set<KnowledgeDocument>();
        public DbSet<KnowledgeChunk> KnowledgeChunks => Set<KnowledgeChunk>();
        public DbSet<VoiceInteraction> VoiceInteractions => Set<VoiceInteraction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("vector");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VoiceAgentDbContext).Assembly);
        }
    }
}
