using Controllers.DTOs;
using Domain.Entities.ChatAgregado;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.LLM.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Connectors.Google.Core;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using System.Net.Http.Json;
using System.Tests;
using WireMock.FluentAssertions;
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
        [Test, Timeout(15000)]
        public async Task Mensaje_SinHerramienta_Test()
        {
            // Arrange
            using var localLLMServer = WireMockServer.Start();
            using var chatServer = WireMockServer.Start();

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
  ]
}"));

            chatServer.Given(Request.Create().WithPath("/Chat").UsingPost())
                .RespondWith(Response.Create().WithSuccess());

            var apiFactory = CreateAPIFactory(
                localLLMServer.Port,
                chatServer.Port);

            var client = apiFactory.CreateClient();

            var mensajeDTO = new MensajeTextoPrueba
            {
                ChatId = Guid.NewGuid(),
                DateTime = DateTime.Now,
                Texto = "Hola"
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/Test", mensajeDTO)
                .ConfigureAwait(false);

            var dbContext = apiFactory.Services.CreateScope().ServiceProvider
                .GetRequiredService<ChatContext>();

            while (dbContext.Set<Mensaje>().Count() < 2)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }

            // Assert
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

            while (chatServer.FindLogEntries(
                    Request.Create().WithPath("/Chat").UsingPost())
                    .Count ==
                0)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }

            chatServer.Should()
                .HaveReceivedACall()
                .AtUrl($"http://localhost:{chatServer.Port}/Chat")
                .And
                .WithBody(
                    new RegexMatcher("\"texto\":\\s*\"Hola soy el test\""));
        }

        [Test, Timeout(15000)]
        public async Task Mensaje_InformacionTool_Test()
        {
            // Arrange
            using var localLLMServer = WireMockServer.Start();
            using var chatServer = WireMockServer.Start();

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

            // Mock llamada de herramienta de información
            localLLMServer
                .Given(
                    Request.Create()
                        .WithPath("/v1/chat/completions")
                        .WithBody(
                            new JsonPathMatcher(
                                    "$.messages[?(@.content == 'Chat')]",
                                    "$.messages[?(@.content == 'Que es una orden de trabajo')]",
                                    "$.tools"))
                        .UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithSuccess()
                        .WithBody(
                            @"{
  ""id"": ""chatcmpl-123"",
  ""object"": ""chat.completion"",
  ""created"": 1741570283,
  ""model"": ""gpt-4"",
  ""choices"": [
    {
      ""index"": 0,
      ""message"": {
        ""role"": ""assistant"",
        ""content"": null,
        ""tool_calls"": [
         {
            ""id"": ""call_abc123"",
            ""type"": ""function"",
            ""function"": {
                ""name"": ""buscar-informacion"",
                ""arguments"": ""{\""pregunta\"":\""Que es una orden de trabajo en CAPATAZ\""}""
          }
         }
        ]
      },
      ""logprobs"": null,
      ""finish_reason"": ""tool_calls""
    }
  ]
}"));

            // Mock respuesta con datos de la herramienta
            localLLMServer
            .Given(
                Request.Create()
                    .WithPath("/v1/chat/completions")
                    .WithBody(
                        new JsonPathMatcher("$.messages[?(@.role == 'tool')]"))
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
              ]
            }"));

            chatServer.Given(Request.Create().WithPath("/Chat").UsingPost())
                .RespondWith(Response.Create().WithSuccess());

            var apiFactory = CreateAPIFactory(
                localLLMServer.Port,
                chatServer.Port);

            var client = apiFactory.CreateClient();

            var mensajeDTO = new MensajeTextoPrueba
            {
                ChatId = Guid.NewGuid(),
                DateTime = DateTime.Now,
                Texto = "Que es una orden de trabajo"
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/Test", mensajeDTO)
                .ConfigureAwait(false);

            var dbContext = apiFactory.Services.CreateScope().ServiceProvider
                .GetRequiredService<ChatContext>();

            while (dbContext.Set<Mensaje>().Count() < 2)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }

            // Assert
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
                                    m => m.Texto ==
                                                "Que es una orden de trabajo"));
                    Assert.That(
                        mensajeIA,
                        Is.TypeOf<MensajeIA>().And
                                .Matches<MensajeIA>(
                                    m => m.Texto == "Hola soy el test"));
                });

            while (chatServer.FindLogEntries(
                    Request.Create().WithPath("/Chat").UsingPost())
                    .Count ==
                0)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }

            chatServer.Should()
                .HaveReceivedACall()
                .AtUrl($"http://localhost:{chatServer.Port}/Chat");
        }
    }
}