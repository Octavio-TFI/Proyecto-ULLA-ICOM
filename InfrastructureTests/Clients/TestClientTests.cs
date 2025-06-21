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
            string mensajePlataformaId = "mensajePlataformaId";

            var mensaje = new MensajeIA
            {
                Texto = "Texto",
                DateTime = DateTime.Now
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
                    new HttpResponseMessage
                    {
                        StatusCode = httpStatusCode,
                        Content = new StringContent(mensajePlataformaId)
                    });

            var httpClient = new HttpClient(
                httpMessageRequestHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient(Platforms.Test))
                .Returns(httpClient);

            var testClient = new TestClient(httpClientFactoryMock.Object);

            // Act and Assert
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
                string reponse = await testClient.EnviarMensajeAsync(
                    chatPlataformaId,
                    usuarioId,
                    mensaje);

                Assert.That(reponse, Is.EqualTo(mensajePlataformaId));
            }
        }

        [Test]
        public void EnviarMensajeTextoAsync_ExceptionOccurredTest()
        {
            // Arrange
            string chatPlataformaId = "chatPlataformaId";
            string usuarioId = "usuarioId";

            var mensaje = new MensajeIA
            {
                Texto = "Texto",
                DateTime = DateTime.Now
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
