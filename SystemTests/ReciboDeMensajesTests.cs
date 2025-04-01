using AppServices.Abstractions;
using Controllers.DTOs;
using Domain.Entities.ChatAgregado;
using Domain.Entities.ConsultaAgregado;
using Domain.Entities.DocumentoAgregado;
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
    public class ReciboDeMensajesTests
        : BaseTests
    {
        [Test, Timeout(15000)]
        public async Task Mensaje_SinHerramienta_Test()
        {
            // Arrange
            using var localLLMServer = WireMockServer.Start();
            using var chatServer = WireMockServer.Start();

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
        public async Task Mensaje_ConHistoriaYSinHerramienta_Test()
        {
            // Arrange
            using var localLLMServer = WireMockServer.Start();
            using var chatServer = WireMockServer.Start();

            var apiFactory = CreateAPIFactory(
                localLLMServer.Port,
                chatServer.Port);

            Guid chatId = Guid.NewGuid();

            // Agregar historial de mensajes
            var context = apiFactory.Services.CreateScope().ServiceProvider
                .GetRequiredService<ChatContext>();

            var chat = new Chat
            {
                Id = chatId,
                ChatPlataformaId = chatId.ToString(),
                Plataforma = Platforms.Test,
                UsuarioId = chatId.ToString(),
            };

            chat.Mensajes
                .AddRange(
                    [new MensajeTextoUsuario
                    {
                        Texto = "Hola",
                        DateTime = DateTime.Now
                    }, new MensajeIA
                    {
                        Texto = "Hola soy el test",
                        DateTime = DateTime.Now
                    }]);

            context.Chats.Add(chat);
            context.SaveChanges();
            context.Dispose();

            localLLMServer
                .Given(
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
            ""content"": ""Hola""
        },
        {   
            ""role"": ""assistant"",
            ""content"": ""Hola soy el test""
        },
        {
            ""role"": ""user"",
            ""content"": ""Como andas?""
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
        ""content"": ""Chau"",
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

            var client = apiFactory.CreateClient();

            var mensajeDTO = new MensajeTextoPrueba
            {
                ChatId = chatId,
                DateTime = DateTime.Now,
                Texto = "Como andas?"
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/Test", mensajeDTO)
                .ConfigureAwait(false);

            var dbContext = apiFactory.Services.CreateScope().ServiceProvider
                .GetRequiredService<ChatContext>();

            while (dbContext.Set<Mensaje>().Count() < 4)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }

            // Assert
            var chatDb = dbContext.Chats.Include(c => c.Mensajes).FirstOrDefault();
            var mensajeUsuario = chatDb?.Mensajes.OrderBy(m => m.DateTime)
                .SkipLast(1)
                .LastOrDefault();
            var mensajeIA = chatDb?.Mensajes.OrderBy(m => m.DateTime)
                .LastOrDefault();

            Assert.Multiple(
                () =>
                {
                    Assert.That(httpResponse.IsSuccessStatusCode);
                    Assert.That(chatDb, Is.Not.Null);
                    Assert.That(
                        mensajeUsuario,
                        Is.TypeOf<MensajeTextoUsuario>().And
                                .Matches<MensajeTextoUsuario>(
                                    m => m.Texto == "Como andas?"));
                    Assert.That(
                        mensajeIA,
                        Is.TypeOf<MensajeIA>().And
                                .Matches<MensajeIA>(
                                    m => m.Texto == "Chau"));
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
                    new RegexMatcher("\"texto\":\\s*\"Chau\""));
        }

        [Test, Timeout(15000)]
        public async Task Mensaje_InformacionTool_Test()
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
            ""role"": ""user"",
            ""content"": ""Que es una orden de trabajo""
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

            chatServer.Given(Request.Create().WithPath("/Chat").UsingPost())
                .RespondWith(Response.Create().WithSuccess());

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

            context = apiFactory.Services.CreateScope().ServiceProvider
                .GetRequiredService<ChatContext>();

            while (context.Set<Mensaje>().Count() < 2)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }

            // Assert
            var chat = context.Chats.Include(c => c.Mensajes).FirstOrDefault();
            var mensajeUsuario = chat?.Mensajes.OrderBy(m => m.DateTime)
                .FirstOrDefault();
            var mensajeIA = chat?.Mensajes.OrderBy(m => m.DateTime)
                .Skip(1)
                .FirstOrDefault();

            context.Entry((MensajeIA)mensajeIA!)
                .Collection(m => m.DocumentosRecuperados)
                .Load();

            context.Entry((MensajeIA)mensajeIA!)
                .Collection(m => m.ConsultasRecuperadas)
                .Load();

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
                                    m => m.Texto == "Hola soy el test" &&
                                                m.DocumentosRecuperados
                                                    .Any(
                                                        d => d.DocumentoId ==
                                                                            documentoId &&
                                                                            d.Rank) &&
                                                m.ConsultasRecuperadas
                                                    .Any(
                                                        c => c.ConsultaId ==
                                                                            consultaId &&
                                                                            !c.Rank)));
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