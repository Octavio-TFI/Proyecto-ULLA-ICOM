using Domain.Abstractions;
using Domain.Entities.ChatAgregado;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Mock;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.EventHandlers.Tests
{
    internal class MensajeRecibidoHandlerTests
    {
        [Test]
        public async Task HandleTest()
        {
            // Arrange
            var chatRepositoryMock = new Mock<IChatRepository>();
            var agentMock = new Mock<IAgent>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<MensajeGeneradoHandler>>();

            var cancellationToken = new CancellationToken();

            var chatId = Guid.NewGuid();
            var chatMock = new Mock<Chat>();

            var respuesta = new MensajeTextoUsuario
            {
                Texto = "Hola",
                DateTime = DateTime.Now
            };

            chatRepositoryMock.Setup(
                repo => repo.GetWithUltimosMensajesAsync(chatId))
                .ReturnsAsync(chatMock.Object);

            chatMock.Setup(x => x.GenerarMensajeAsync(agentMock.Object))
                .ReturnsAsync(respuesta);

            var handler = new MensajeRecibidoHandler(
                chatRepositoryMock.Object,
                agentMock.Object,
                unitOfWorkMock.Object,
                loggerMock.Object);

            var notification = new MensajeRecibidoEvent { EntityId = chatId };

            // Act
            await handler.Handle(notification, cancellationToken)
                .ConfigureAwait(false);

            // Assert
            chatMock.Verify(x => x.GenerarMensajeAsync(agentMock.Object), Times.Once);

            unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);

            loggerMock.VerifyLog()
                .InformationWasCalled()
                .MessageEquals(
                    $@"
MENSAJE GENERADO
Texto: {respuesta}
ChatId: {notification.EntityId}")
                .Times(1);
        }
    }
}
