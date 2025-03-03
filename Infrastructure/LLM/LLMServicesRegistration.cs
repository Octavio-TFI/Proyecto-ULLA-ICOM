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
            services
                .AddSingleton<IEmbeddingService, LMStudioEmbeddingService>();

            services.AddHttpClient<IEmbeddingService, LMStudioEmbeddingService>(
                (services, httpClient) =>
                {
                    string url = services.GetRequiredService<IConfiguration>()
                                .GetValue<string>("URLs:LLMLocal") ??
                        throw new Exception(
                                "Se debe configurar URL del LLM Local en URLs:LLMLocal");

                    httpClient.BaseAddress =
                        new Uri($"{url}/v1/embeddings");
                });

            services.AddKeyedTransient(
                TipoLLM.Pequeño,
                (services, key) =>
                {
                    string url = services.GetRequiredService<IConfiguration>()
                                .GetValue<string>("URLs:LLMLocal") ??
                        throw new Exception(
                                "Se debe configurar URL del LLM Local en URLs:LLMLocal");

                    var openAiClient = new OpenAIClient(
                        new ApiKeyCredential("lm-studio"),
                        new OpenAIClientOptions
                        {
                            Endpoint = new Uri($"{url}/v1")
                        });

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
                    HttpClient? httpClient = null;
                    string? url = services.GetRequiredService<IConfiguration>()
                        .GetValue<string>("URLs:LLMNube");

                    // Para poder testear sin ir a la nube
                    if (url is not null)
                    {
                        var proxy = new WebProxy(url);

                        httpClient = new HttpClient(
                            new HttpClientHandler { Proxy = proxy });
                    }

                    string? apiKey = services
                        .GetRequiredService<IConfiguration>()
                                .GetValue<string>("GeminiApiKey") ??
                        throw new Exception("GeminiApiKey no configurada");

                    var kernelBuilder = Kernel.CreateBuilder()
                        .AddGoogleAIGeminiChatCompletion(
                            "gemini-2.0-flash",
                            apiKey,
                            httpClient: httpClient);

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
