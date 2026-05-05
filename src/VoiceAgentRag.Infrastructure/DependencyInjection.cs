using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Application.Abstractions.AI;
using VoiceAgentRag.Application.Abstractions.Persistence;
using VoiceAgentRag.Application.Abstractions.Rag;
using VoiceAgentRag.Infrastructure.AI;
using VoiceAgentRag.Infrastructure.Persistence;
using VoiceAgentRag.Infrastructure.Persistence.Repositories;
using VoiceAgentRag.Infrastructure.Rag;

namespace VoiceAgentRag.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

            services.AddDbContext<VoiceAgentDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddScoped<ITextChunker, SimpleTextChunker>();
            services.AddScoped<IRagService, SimpleRagService>();

            services.AddScoped<IIntentDetector, FakeIntentDetector>();
            services.AddScoped<IAnswerGenerator, FakeAnswerGenerator>();

            return services;
        }
    }
}
