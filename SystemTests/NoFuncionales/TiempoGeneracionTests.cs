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
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace System.Tests.NoFuncionales
{
    public class TiempoGeneracionTests
        : BaseTests
    {
        [Test, Timeout(3 * 60 * 1000)]
        public async Task GeneracionRespuestaMenos3MinutosTest()
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
                        .WithDelay(TimeSpan.FromSeconds(1))
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
            ""role"": ""user"",
            ""content"": ""Que es una orden de trabajo""
        }
    ]
}"))
                    .UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithSuccess()
                        .WithDelay(TimeSpan.FromSeconds(1))
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
                        .WithDelay(TimeSpan.FromSeconds(10))
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
                        .WithDelay(TimeSpan.FromSeconds(10))
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
            // Teniendo en cuenta la generacion de 4mil tokens a 250 tokens por segundo (Velocidad reportada del LLM)
            var tokenGenerationDelay = TimeSpan.FromSeconds(4000.0d / 250.0d);

            localLLMServer.Given(
                Request.Create()
                    .WithPath("/v1/chat/completions")
                    .WithBody(
                        new JsonPathMatcher("$.messages[?(@.role == 'tool')]"))
                    .UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithSuccess()
                        .WithDelay(tokenGenerationDelay)
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

            var mensajeDTO = new TestMensajeTexto
            {
                ChatId = Guid.NewGuid(),
                DateTime = DateTime.Now,
                Texto = "Que es una orden de trabajo"
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync(
                "/Test/texto",
                mensajeDTO)
                .ConfigureAwait(false);

            while (apiFactory.Services.CreateScope().ServiceProvider
                    .GetRequiredService<ChatContext>()
                    .Set<Mensaje>()
                    .Count() <
                2)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }
        }
    }
}
