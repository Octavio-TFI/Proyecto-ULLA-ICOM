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
            var handler = new MensajeRecibidoHandler(
                mensajeRepositoryMock.Object,
                generadorRespuestaMock.Object);

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
        }
    }
}
