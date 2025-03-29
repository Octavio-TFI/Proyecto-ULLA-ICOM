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
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM
{
    public static class LLMServicesRegistration
    {
        public static IServiceCollection AddLLMServices(
            this IServiceCollection services)
        {
            services.AddSingleton<IEmbeddingService, LMStudioEmbeddingService>();

            services.AddHttpClient<IEmbeddingService, LMStudioEmbeddingService>(
                (services, httpClient) =>
                {
                    string url = services.GetRequiredService<IConfiguration>()
                                .GetValue<string>("LLMLocal:URL") ??
                        throw new Exception(
                                "Se debe configurar URL del LLM Local en LLMLocal:URL");

                    httpClient.BaseAddress =
                        new Uri($"{url}/v1/embeddings");
                });

            services.AddKeyedTransient(
                TipoLLM.Pequeño,
                (services, key) =>
                {
                    var config = services.GetRequiredService<IConfiguration>();

                    string url = config.GetValue<string>("LLMLocal:URL") ??
                        throw new Exception(
                                "Se debe configurar URL del LLM Local en LLMLocal:URL");

                    var openAiClient = new OpenAIClient(
                        new ApiKeyCredential("lm-studio"),
                        new OpenAIClientOptions
                        {
                            Endpoint = new Uri($"{url}/v1")
                        });

                    string model = config.GetValue<string>("LLMLocal:Model") ??
                        throw new Exception(
                                "Se debe configurar el modelo del LLM Local en LLMLocal:Model");

                    var kernelBuilder = Kernel.CreateBuilder()
                        .AddOpenAIChatCompletion(
                            model,
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
                    var config = services.GetRequiredService<IConfiguration>();

                    bool testing = config.GetValue<bool>("Testing");

                    var kernelBuilder = Kernel.CreateBuilder();

                    if (testing)
                    {
                        string url = config.GetValue<string>("LLMLocal:URL") ??
                            throw new Exception(
                                "Se debe configurar URL del LLM Local en LLMLocal:URL");

                        var openAiClient = new OpenAIClient(
                            new ApiKeyCredential("lm-studio"),
                            new OpenAIClientOptions
                            {
                                Endpoint = new Uri($"{url}/v1")
                            });

                        string model = config.GetValue<string>("LLMLocal:Model") ??
                            throw new Exception(
                                "Se debe configurar el modelo del LLM Local en LLMLocal:Model");

                        kernelBuilder.AddOpenAIChatCompletion(
                            model,
                            openAiClient);

                        kernelBuilder.Services
                            .AddSingleton<IExecutionSettingsFactory, OpenAiExecutionSettingsFactory>(
                                );
                    }
                    else
                    {
                        string apiKey = config.GetValue<string>("LLMGoogle:ApiKey") ??
                            throw new Exception(
                                "Se debe configurar GeminiApiKey en LLMGoogle:ApiKey");

                        string model = config.GetValue<string>("LLMGoogle:Model") ??
                            throw new Exception(
                                "Se debe configurar GeminiModel en LLMGoogle:Model");

                        kernelBuilder.AddGoogleAIGeminiChatCompletion(
                            model,
                            apiKey);

                        kernelBuilder.Services
                            .AddSingleton<IExecutionSettingsFactory, GeminiExecutionSettingsFactory>(
                                );
                    }

                    return kernelBuilder.Build();
                });

            services.AddTransient<IAgentBuilder, AgentBuilder>();
            services.AddSingleton<IChatHistoryAdapter, ChatHistoryAdapter>();

            return services;
        }
    }
}
