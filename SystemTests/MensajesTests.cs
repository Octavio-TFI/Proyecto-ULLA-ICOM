using Controllers.DTOs;
using Domain.Entities.ChatAgregado;
using Infrastructure.Database;
using Infrastructure.LLM.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Connectors.Google.Core;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using System.Net.Http.Json;
using System.Tests;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Util;

namespace SystemTests
{
    public class MensajesTests
        : BaseTests
    {
        [Test, Timeout(11000)]
        public async Task MensajeBasicoTest()
        {
            // Arrange
            var localLLMServer = WireMockServer.Start();
            var chatServer = WireMockServer.Start();

            localLLMServer
                .Given(Request.Create().WithPath("/v1/embeddings").UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithSuccess()
                        .WithBodyAsJson(
                            new EmbeddingResponseList
                                {
                                    Data =
                                        [..Enumerable.Repeat(
                                                    new EmbeddingResponse
                                        {
                                            Embedding = [1,2,3]
                                        },
                                                    10)]
                                }));

            localLLMServer
                .Given(
                    Request.Create()
                        .WithPath("/v1/chat/completions")
                        .WithBody(
                            new JsonPathMatcher(
                                    "$.messages[?(@.role == 'user' && @.content == 'Hola')]"))
                        .UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithSuccess()
                        .WithBody(
                            @"{
  ""id"": ""chatcmpl-B9MHDbslfkBeAs8l4bebGdFOJ6PeG"",
  ""object"": ""chat.completion"",
  ""created"": 1741570283,
  ""model"": ""gpt-4o-2024-08-06"",
  ""choices"": [
    {
      ""index"": 0,
      ""message"": {
        ""role"": ""assistant"",
        ""content"": ""Hola soy el test"",
        ""refusal"": null,
        ""annotations"": []
      },
      ""logprobs"": null,
      ""finish_reason"": ""stop""
    }
  ],
  ""usage"": {
    ""prompt_tokens"": 1117,
    ""completion_tokens"": 46,
    ""total_tokens"": 1163,
    ""prompt_tokens_details"": {
      ""cached_tokens"": 0,
      ""audio_tokens"": 0
    },
    ""completion_tokens_details"": {
      ""reasoning_tokens"": 0,
      ""audio_tokens"": 0,
      ""accepted_prediction_tokens"": 0,
      ""rejected_prediction_tokens"": 0
    }
  },
  ""service_tier"": ""default"",
  ""system_fingerprint"": ""fp_fc9f1d7035""
}"));

            var apiFactory = CreateAPIFactory(
                localLLMServer.Port);

            var client = apiFactory.CreateClient();

            var mensajeDTO = new MensajeTextoPrueba
            {
                ChatId = Guid.NewGuid(),
                DateTime = DateTime.Now,
                Texto = "Hola"
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/Test", mensajeDTO);

            // Assert
            var dbContext = apiFactory.Services.CreateScope().ServiceProvider
                .GetRequiredService<ChatContext>();

            while (dbContext.Set<Mensaje>().Count() < 2)
            {
                await Task.Delay(100);
            }

            var chat = dbContext.Chats.Include(c => c.Mensajes).FirstOrDefault();
            var mensajeUsuario = chat?.Mensajes.OrderBy(m => m.DateTime)
                .FirstOrDefault();
            var mensajeIA = chat?.Mensajes.OrderBy(m => m.DateTime)
                .Skip(1)
                .FirstOrDefault();

            Assert.Multiple(
                () =>
                {
                    Assert.That(httpResponse.IsSuccessStatusCode);
                    Assert.That(chat, Is.Not.Null);
                    Assert.That(
                        mensajeUsuario,
                        Is.TypeOf<MensajeTextoUsuario>().And
                                .Matches<MensajeTextoUsuario>(
                                    m => m.Texto == "Hola"));
                    Assert.That(
                        mensajeIA,
                        Is.TypeOf<MensajeIA>().And
                                .Matches<MensajeIA>(
                                    m => m.Texto == "Hola soy el test"));
                });
        }
    }
}