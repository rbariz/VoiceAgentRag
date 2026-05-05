using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoiceAgentRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKnowledgeChunkEmbedding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS vector;");

            migrationBuilder.Sql("""
            ALTER TABLE knowledge_chunks
            ADD COLUMN IF NOT EXISTS embedding vector(768);
        """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            ALTER TABLE knowledge_chunks
            DROP COLUMN IF EXISTS embedding;
        """);
        }
    }
}
