using Controllers.DTOs;
using Domain.Entities.ChatAgregado;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Tests;
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