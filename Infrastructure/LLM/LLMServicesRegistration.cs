using AppServices;
using AppServices.Ports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using OpenAI;
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

            var openAiClient = new OpenAIClient(
                new ApiKeyCredential("lm-studio"),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri("http://ulaai.ulanet.local:1234/v1")
                });

            var kernel = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(
                    "meta-llama-3.1-8b-instruct",
                    openAiClient)
                .Build();

            var rankerPromptsDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Prompts",
                "Ranker");

            var rankerPlugin = kernel.ImportPluginFromPromptDirectory(
                rankerPromptsDirectory,
                promptTemplateFactory: new KernelPromptTemplateFactory());

            services.AddKeyedSingleton(TipoKernel.Ranker, kernel);

            services.AddSingleton<IGeneradorRespuesta, GeneradorRespuesta>();

            return services;
        }
    }
}
