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
        public async void HandleTest()
        {
            // Arrange
            var msjGeneradoEvent = new MensajeGeneradoEvent
            {
                Mensaje =
                    new MensajeTexto
                    {
                        ChatId = 1,
                        Texto = "Texto",
                        DateTime = DateTime.Now,
                        Tipo = TipoMensaje.Asistente
                    }
            };

            var chat = new Chat
            {
                Id = 1,
                ChatPlataformaId = "1",
                UsuarioId = "1",
                Plataforma = "Test"
            };

            var chatRepositoryMock = new Mock<IChatRepository>();
            var clientFactoryMock = new Mock<Func<string, IClient>>();
            var clientMock = new Mock<IClient>();


            var handler = new MensajeGeneradoHandler(
                chatRepositoryMock.Object,
                clientFactoryMock.Object);

            // Act
            await handler.Handle(msjGeneradoEvent, CancellationToken.None);

            // Assert
            clientMock.Verify(
                x => x.EnviarMensaje(
                    chat.ChatPlataformaId,
                    chat.UsuarioId,
                    msjGeneradoEvent.Mensaje),
                Times.Once);
        }
    }
}
