using AppServices;
using Domain.Abstractions;
using Domain.ValueObjects;
using Infrastructure.LLM.Abstractions;
using Infrastructure.LLM.ExecutionSettingsFactories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
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
            var openAiClient = new OpenAIClient(
                new ApiKeyCredential("lm-studio"),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri("http://ulaai.ulanet.local:1234/v1")
                });

            services
                .AddSingleton<IEmbeddingService, LMStudioEmbeddingService>(
                    );

            services.AddHttpClient<IEmbeddingService, LMStudioEmbeddingService>(
                x => x.BaseAddress =
                    new Uri("http://ulaai.ulanet.local:1234/v1/embeddings"));

            services.AddKeyedTransient(
                TipoLLM.Pequeño,
                (services, key) =>
                {
                    var kernelBuilder = Kernel.CreateBuilder()
                        .AddOpenAIChatCompletion(
                            "qwen2.5-14b-instruct",
                            openAiClient);

                    kernelBuilder.Services
                        .AddSingleton<IExecutionSettingsFactory, OpenAiExecutionSettingsFactory>(
                            );

                    return kernelBuilder.Build();
                });

            services.AddKeyedTransient(
                TipoLLM.Grande,
                (services, key) =>
                {
                    string? apiKey = services
                        .GetRequiredService<IConfiguration>()
                                .GetValue<string>("GeminiApiKey") ??
                        throw new Exception("GeminiApiKey no configurada");

                    var kernelBuilder = Kernel.CreateBuilder()
                        .AddGoogleAIGeminiChatCompletion(
                            "gemini-2.0-flash",
                            apiKey);

                    kernelBuilder.Services
                        .AddSingleton<IExecutionSettingsFactory, GeminiExecutionSettingsFactory>(
                            );

                    return kernelBuilder.Build();
                });

            services.AddTransient<IAgentBuilder, AgentBuilder>();
            services.AddSingleton<IChatHistoryAdapter, ChatHistoryAdapter>();

            return services;
        }
    }
}
