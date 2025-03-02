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
            var msjGeneradoEvent = new MensajeGeneradoEvent
            {
                EntityId = Guid.NewGuid(),
                Mensaje =
                    new MensajeTextoUsuario
                    {
                        Texto = "Texto",
                        DateTime = DateTime.Now
                    }
            };

            var chat = new Chat
            {
                ChatPlataformaId = "1",
                UsuarioId = "1",
                Plataforma = "Test"
            };

            var chatRepositoryMock = new Mock<IChatRepository>();
            var loggerMock = new Mock<ILogger<MensajeGeneradoHandler>>();
            var clientFactoryMock = new Mock<Func<string, IClient>>();
            var clientMock = new Mock<IClient>();

            chatRepositoryMock
                .Setup(x => x.GetAsync(msjGeneradoEvent.EntityId))
                .ReturnsAsync(chat);

            clientFactoryMock.Setup(x => x.Invoke("Test"))
                .Returns(clientMock.Object);

            var handler = new MensajeGeneradoHandler(
                chatRepositoryMock.Object,
                clientFactoryMock.Object,
                loggerMock.Object);

            // Act
            await handler.Handle(msjGeneradoEvent, CancellationToken.None)
                .ConfigureAwait(false);

            // Assert
            clientMock.Verify(
                x => x.EnviarMensajeAsync(
                    chat.ChatPlataformaId,
                    chat.UsuarioId,
                    msjGeneradoEvent.Mensaje),
                Times.Once);

            loggerMock.VerifyLog()
                .InformationWasCalled()
                .MessageEquals(
                    $@"
MENSAJE ENVIADO
Texto: {msjGeneradoEvent.Mensaje}
ChatId: {msjGeneradoEvent.EntityId}")
                .Times(1);
        }
    }
}
