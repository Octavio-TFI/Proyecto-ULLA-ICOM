using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Clients.Tests
{
    internal class TestClientTests
    {
        [Test]
        public async Task EnviarMensajeTextoAsyncTest()
        {
            // Arrange
            var mensaje = new MensajeTexto
            {
                ChatId = 1,
                Texto = "Texto",
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Asistente
            };

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();

            var testClient = new TestClient(httpClientFactoryMock.Object);

            // Act
            await testClient.EnviarMensajeAsync(
                "chatPlataformaId",
                "usuarioId",
                mensaje);

            // Assert
            Assert.Fail();
        }
    }
}
