﻿using AppServices;
using AppServices.Ports;
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

            services.AddTransient(
                services =>
                {
                    var kernelBuilder = Kernel.CreateBuilder()
                        .AddOpenAIChatCompletion(
                            "meta-llama-3.1-8b-instruct",
                            openAiClient);

                    kernelBuilder.Services
                        .AddSingleton<ITextEmbeddingGenerationService, LMStudioTextEmbeddingGenerationService>(
                            );

                    kernelBuilder.Services
                        .AddHttpClient<ITextEmbeddingGenerationService, LMStudioTextEmbeddingGenerationService>(
                            x => x.BaseAddress =
                                    new Uri(
                                        "http://ulaai.ulanet.local:1234/v1/embeddings"));


                    return kernelBuilder.Build();
                });

            return services;
        }
    }
}
