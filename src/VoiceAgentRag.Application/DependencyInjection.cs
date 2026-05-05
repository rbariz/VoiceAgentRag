using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAgentRag.Application.Conversations;
using VoiceAgentRag.Application.Knowledge;
using VoiceAgentRag.Application.Voice;

namespace VoiceAgentRag.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IVoiceAgentService, VoiceAgentService>();

            services.AddScoped<IKnowledgeIngestionService, KnowledgeIngestionService>();

            services.AddScoped<IConversationQueryService, ConversationQueryService>();

            return services;
        }
    }
}
