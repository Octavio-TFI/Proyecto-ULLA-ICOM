using Controllers.DTOs;
using Domain.Entities.ChatAgregado;
using Infrastructure.Database;
using Infrastructure.LLM.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Tests;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace SystemTests
{
    public class MensajesTests
        : BaseTests
    {
        [Test]
        public async Task MensajeBasicoTest()
        {
            // Arrange
            var localLLMServer = WireMockServer.Start();

            localLLMServer
                .Given(Request.Create().WithPath("/v1/embeddings").UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithSuccess()
                        .WithBodyAsJson(
                            new EmbeddingResponseList
                                {
                                    Data = [new() { Embedding = [1,2,3] }]
                                }));

            var nubeLLMServer = WireMockServer.Start();

            var apiFactory = CreateAPIFactory(
                localLLMServer.Port,
                nubeLLMServer.Port);

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

            var chat = dbContext.Chats.Include(c => c.Mensajes).FirstOrDefault();
            var mensaje = chat?.Mensajes.FirstOrDefault();

            Assert.Multiple(
                () =>
                {
                    Assert.That(httpResponse.IsSuccessStatusCode);
                    Assert.That(chat, Is.Not.Null);
                    Assert.That(
                        mensaje,
                        Is.TypeOf<MensajeTextoUsuario>().And
                                .Matches<MensajeTextoUsuario>(
                                    m => m.Texto == "Hola"));
                });
        }
    }
}