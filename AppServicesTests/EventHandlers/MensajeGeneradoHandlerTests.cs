using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.EventHandlers.Tests
{
    internal class MensajeGeneradoHandlerTests
    {
        [Test]
        public async Task HandleTest()
        {
            // Arrange
            var chat = new Chat
            {
                ChatPlataformaId = "1",
                UsuarioId = "1",
                Plataforma = "Test"
            };

            var mensaje = new MensajeIA
            {
                Texto = "Hola",
                DateTime = DateTime.Now,
            };

            var msjGeneradoEvent = new MensajeGeneradoEvent
            {
                EntityId = chat.Id,
                MensajeId = mensaje.Id,
            };

            var chatRepositoryMock = new Mock<IChatRepository>();
            var mensajeIARepositoryMock = new Mock<IMensajeIARepository>();
            var loggerMock = new Mock<ILogger<MensajeGeneradoHandler>>();
            var clientFactoryMock = new Mock<Func<string, IClient>>();
            var clientMock = new Mock<IClient>();

            chatRepositoryMock
                .Setup(x => x.GetAsync(msjGeneradoEvent.EntityId))
                .ReturnsAsync(chat);

            mensajeIARepositoryMock
                .Setup(x => x.GetAsync(msjGeneradoEvent.MensajeId))
                .ReturnsAsync(mensaje);

            clientMock.Setup(
                x => x.EnviarMensajeAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<MensajeIA>()))
                .ReturnsAsync("2");

            clientFactoryMock.Setup(x => x.Invoke("Test"))
                .Returns(clientMock.Object);

            var handler = new MensajeGeneradoHandler(
                chatRepositoryMock.Object,
                mensajeIARepositoryMock.Object,
                clientFactoryMock.Object,
                loggerMock.Object);

            // Act
            await handler.Handle(msjGeneradoEvent, CancellationToken.None)
                .ConfigureAwait(false);

            // Assert
            Assert.That(mensaje.PlataformaMensajeId, Is.EqualTo("2"));

            clientMock.Verify(
                x => x.EnviarMensajeAsync(
                    chat.ChatPlataformaId,
                    chat.UsuarioId,
                    mensaje),
                Times.Once);

            loggerMock.VerifyLog()
                .InformationWasCalled()
                .MessageEquals(
                    $@"
MENSAJE ENVIADO
Texto: {mensaje.Texto}
ChatId: {msjGeneradoEvent.EntityId}")
                .Times(1);
        }
    }
}
