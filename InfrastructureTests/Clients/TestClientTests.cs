using AppServices.Abstractions;
using Domain.Exceptions;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Clients.Tests
{
    internal class TestClientTests
    {
        [Test]
        [TestCase(HttpStatusCode.OK, false)]
        [TestCase(HttpStatusCode.BadRequest, true)]
        public async Task EnviarMensajeTextoAsyncTest(
            HttpStatusCode httpStatusCode,
            bool shouldThrow)
        {
            // Arrange
            string chatPlataformaId = "chatPlataformaId";
            string usuarioId = "usuarioId";

            var mensaje = new MensajeTexto
            {
                ChatId = 1,
                Texto = "Texto",
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Asistente
            };

            var httpMessageRequestHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageRequestHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback(
                    async (HttpRequestMessage message, CancellationToken ct) =>
                    {
                        var testDTO = await message.Content!
                            .ReadFromJsonAsync<TestDTO>(ct);

                        Assert.Multiple(
                            () =>
                            {
                                Assert.That(
                                    message.RequestUri?.ToString(),
                                    Is.EqualTo("http://localhost/Chat"));

                                Assert.That(
                                    testDTO?.ChatId,
                                    Is.EqualTo(chatPlataformaId));

                                Assert.That(
                                    testDTO?.Texto,
                                    Is.EqualTo(mensaje.Texto));
                            });
                    })
                .ReturnsAsync(
                    new HttpResponseMessage { StatusCode = httpStatusCode });

            var httpClient = new HttpClient(
                httpMessageRequestHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient(Platforms.Test))
                .Returns(httpClient);

            var testClient = new TestClient(httpClientFactoryMock.Object);

            // Act
            if (shouldThrow)
            {
                Assert.ThrowsAsync<ErrorEnviandoMensajeException>(
                    async () => await testClient.EnviarMensajeAsync(
                        chatPlataformaId,
                        usuarioId,
                        mensaje));
            }
            else
            {
                await testClient.EnviarMensajeAsync(
                    chatPlataformaId,
                    usuarioId,
                    mensaje);
            }
        }

        [Test]
        public async Task EnviarMensajeTextoAsync_ExceptionOccurredTest()
        {
            // Arrange
            string chatPlataformaId = "chatPlataformaId";
            string usuarioId = "usuarioId";

            var mensaje = new MensajeTexto
            {
                ChatId = 1,
                Texto = "Texto",
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Asistente
            };

            var innerEx = new Exception("Exception occurred");

            var httpMessageRequestHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageRequestHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(innerEx);

            var httpClient = new HttpClient(
                httpMessageRequestHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient(Platforms.Test))
                .Returns(httpClient);

            var testClient = new TestClient(httpClientFactoryMock.Object);

            // Act
            var ex = Assert.ThrowsAsync<ErrorEnviandoMensajeException>(
                async () => await testClient.EnviarMensajeAsync(
                    chatPlataformaId,
                    usuarioId,
                    mensaje));

            // Assert
            Assert.That(ex.InnerException, Is.EqualTo(innerEx));
        }
    }
}
