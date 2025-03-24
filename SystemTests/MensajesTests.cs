using Controllers.DTOs;
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

            var mensaje = new MensajeTextoPrueba
            {
                ChatId = Guid.NewGuid(),
                DateTime = DateTime.Now,
                Texto = "Hola"
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/Test", mensaje);

            // Assert
            Assert.That(httpResponse.IsSuccessStatusCode);
        }
    }
}