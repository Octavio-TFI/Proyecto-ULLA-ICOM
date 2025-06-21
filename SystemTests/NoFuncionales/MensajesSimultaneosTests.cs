using AppServices.Abstractions;
using Controllers.DTOs;
using Domain.Entities.ChatAgregado;
using Domain.Entities.ConsultaAgregado;
using Domain.Entities.DocumentoAgregado;
using Infrastructure.Database;
using Infrastructure.LLM.DTOs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WireMock.FluentAssertions;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace System.Tests.NoFuncionales
{
    public class MensajesSimultaneosTests
        : BaseTests
    {
        [Test, Timeout(20000)]
        public async Task MensajesSimultaneos50Test()
        {
            // Arrange
            using var localLLMServer = WireMockServer.Start();
            using var chatServer = WireMockServer.Start();

            var apiFactory = CreateAPIFactory(
                localLLMServer.Port,
                chatServer.Port);

            // Agregar documentacion y consulta
            var context = apiFactory.Services.CreateScope().ServiceProvider
                .GetRequiredService<ChatContext>();

            Guid documentoId = Guid.NewGuid();
            Guid consultaId = Guid.NewGuid();

            context.Documents
                .Add(
                    new Document
                    {
                        Id = documentoId,
                        Texto = "Documento",
                        Filename = "doc1.txt",
                        Chunks =
                            [new()
                                {
                                    Id = Guid.NewGuid(),
                                    Texto = "Texto del documento",
                                    Embedding = [1,2,3]
                                }]
                    });

            context.Consultas
                .Add(
                    new Consulta
                    {
                        Id = consultaId,
                        RemoteId = 1,
                        EmbeddingTitulo = [1, 2, 3],
                        EmbeddingDescripcion = [1, 2, 3],
                        Titulo = "Titulo de la consulta",
                        Descripcion = "Descripcion de la consulta",
                        Solucion = "Solucion"
                    });

            context.SaveChanges();
            context.Dispose();

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
            localLLMServer.Given(
                Request.Create()
                    .WithPath("/v1/chat/completions")
                    .WithBody(
                        new JsonPartialMatcher(
                                @"
{
    ""messages"": [
        {
            ""role"": ""system"",
            ""content"": ""Chat""
        },
        {
            ""role"": ""user""
        }
    ]
}"))
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

            // Mock ranker
            localLLMServer.Given(
                Request.Create()
                    .WithPath("/v1/chat/completions")
                    .WithBody(
                        new JsonPartialMatcher(
                                @"
{
    ""messages"": [
        {
            ""role"": ""system"",
            ""content"": ""Documento""
        },
        {
            ""role"": ""user"",
            ""content"": ""Que es una orden de trabajo en CAPATAZ""
        }
    ]
}"))
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
                    ""content"": ""{\""Score\"": true}"",
                    ""refusal"": null,
                    ""annotations"": []
                  },
                  ""logprobs"": null,
                  ""finish_reason"": ""stop""
                }
              ]
            }"));

            localLLMServer.Given(
                Request.Create()
                    .WithPath("/v1/chat/completions")
                    .WithBody(
                        new JsonPartialMatcher(
                                @"
{
    ""messages"": [
        {
            ""role"": ""system"",
            ""content"": ""^# Titulo(.|\\s)*$""
        },
        {
            ""role"": ""user"",
            ""content"": ""Que es una orden de trabajo en CAPATAZ""
        }
    ]
}",
                                false,
                                true))
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
                    ""content"": ""{\""Score\"": false}"",
                    ""refusal"": null,
                    ""annotations"": []
                  },
                  ""logprobs"": null,
                  ""finish_reason"": ""stop""
                }
              ]
            }"));

            // Mock respuesta con datos de la herramienta
            localLLMServer.Given(
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

            string mensajePlataformaId = Guid.NewGuid().ToString();

            chatServer.Given(Request.Create().WithPath("/Chat").UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithSuccess()
                        .WithBody(mensajePlataformaId));

            var client = apiFactory.CreateClient();

            var mensajes = Enumerable.Range(0, 50)
                .Select(
                    i => new TestMensajeTexto
                    {
                        Texto = $"Mensaje {i + 1}",
                        DateTime = DateTime.Now.AddSeconds(i),
                        ChatId = Guid.NewGuid(),
                    })
                .ToList();

            // Act
            var responsesTasks = mensajes.Select(
                mensaje => client.PostAsJsonAsync("/Test/texto", mensaje))
                .ToList();

            var responses = await Task.WhenAll(responsesTasks);

            // Assert
            Assert.That(
                responses.Select(r => r.IsSuccessStatusCode),
                Is.All.True,
                "Todos los requests deben ser exitosos");

            var updatedDbContext = apiFactory.Services.CreateScope()
                .ServiceProvider
                .GetRequiredService<ChatContext>();

            Assert.That(
                updatedDbContext.Chats.Count(),
                Is.EqualTo(50),
                "Debe haber 50 chats.");


            while (chatServer.FindLogEntries(
                    Request.Create().WithPath("/Chat").UsingPost())
                    .Count <
                50)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }

            chatServer.Should()
                .HaveReceived(50)
                .Calls()
                .AtUrl($"http://localhost:{chatServer.Port}/Chat");
        }
    }
}
