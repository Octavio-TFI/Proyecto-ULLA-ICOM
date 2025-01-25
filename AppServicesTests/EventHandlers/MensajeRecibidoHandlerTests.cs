using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Mock;
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
            var mensajeRepositoryMock = new Mock<IMensajeRepository>();
            var generadorRespuestaMock = new Mock<IGeneradorRespuesta>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<MensajeGeneradoHandler>>();

            var chatId = 1;
            var request = new MensajeRecibidoEvent { ChatId = chatId };
            var cancellationToken = new CancellationToken();

            var mensajes = new List<Mensaje>
            {
                new MensajeTexto
                {
                    ChatId = chatId,
                    Texto = "Hola",
                    DateTime = DateTime.Now,
                    Tipo = TipoMensaje.Usuario
                }
            };

            var respuesta = new MensajeTexto
            {
                ChatId = chatId,
                Texto = "Hola",
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Asistente
            };

            mensajeRepositoryMock.Setup(
                repo => repo.GetUltimosMensajesChatAsync(chatId))
                .ReturnsAsync(mensajes);

            generadorRespuestaMock.Setup(
                gen => gen.GenerarRespuestaAsync(mensajes))
                .ReturnsAsync(respuesta);

            var handler = new MensajeRecibidoHandler(
                mensajeRepositoryMock.Object,
                generadorRespuestaMock.Object,
                unitOfWorkMock.Object,
                loggerMock.Object);

            // Act
            await handler.Handle(request, cancellationToken);

            // Assert
            mensajeRepositoryMock.Verify(
                repo => repo.GetUltimosMensajesChatAsync(chatId),
                Times.Once);

            generadorRespuestaMock.Verify(
                gen => gen.GenerarRespuestaAsync(mensajes),
                Times.Once);

            mensajeRepositoryMock.Verify(
                repo => repo.InsertAsync(respuesta),
                Times.Once);

            unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);

            loggerMock.VerifyLog()
                .InformationWasCalled()
                .MessageEquals(
                    $@"
MENSAJE GENERADO
Texto: {respuesta}
ChatId: {respuesta.ChatId}")
                .Times(1);
        }
    }
}
