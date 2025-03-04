using Controllers.DTOs;
using System.Net.Http.Json;
using System.Tests;

namespace SystemTests
{
    public class ChatTests
    {
        [Test]
        public async Task PrimerMensajeTest()
        {
            // Arrange
            var apiFactory = new APIFactory();

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