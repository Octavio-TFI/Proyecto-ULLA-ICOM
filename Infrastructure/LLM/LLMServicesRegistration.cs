using AppServices.Ports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM
{
    public static class LLMServicesRegistration
    {
        public static IServiceCollection AddLLMServices(
            this IServiceCollection services)
        {
            services.AddSingleton<ITextEmbeddingGenerationService, LMStudioTextEmbeddingGenerationService>(
                );

            services.AddHttpClient<ITextEmbeddingGenerationService, LMStudioTextEmbeddingGenerationService>(
                x => x.BaseAddress =
                    new Uri("http://ulaai.ulanet.local:1234/v1/embeddings"));

            services.AddSingleton<IGeneradorRespuesta, GeneradorRespuesta>();

            return services;
        }
    }
}
