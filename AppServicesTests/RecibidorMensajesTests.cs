using Domain.Entities.ChatAgregado;
using Moq;

namespace AppServices.Tests
{
    [TestFixture()]
    public class RecibidorMensajesTests
    {
        [Test()]
        public async Task RecibirMensajeTextoAsync_ChatExistsTest()
        {
            // Arrange
            var mensajeRecibidoDTO = new MensajeTextoRecibidoDTO
            {
                ChatPlataformaId = "chat2",
                DateTime = DateTime.Now,
                Plataforma = "Test",
                Texto = "Hola",
                UsuarioId = "usuario"
            };

            var chatMock = new Mock<Chat>();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var chatRepositoryMock = new Mock<IChatRepository>();

            chatRepositoryMock.SetupSequence(
                r => r.GetAsync(
                    mensajeRecibidoDTO.UsuarioId,
                    mensajeRecibidoDTO.ChatPlataformaId,
                    mensajeRecibidoDTO.Plataforma))
                .ReturnsAsync(chatMock.Object);

            var recibidorMensajes = new RecibidorMensajes(
                unitOfWorkMock.Object,
                chatRepositoryMock.Object);

            // Act
            await recibidorMensajes.RecibirMensajeTextoAsync(mensajeRecibidoDTO);

            // Assert
            chatMock.Verify(
                x => x.AñadirMensajeTextoRecibido(
                    mensajeRecibidoDTO.DateTime,
                    mensajeRecibidoDTO.Texto),
                Times.Once);

            chatRepositoryMock.Verify(
                r => r.InsertAsync(It.IsAny<Chat>()),
                Times.Never);

            unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task RecibirMensajeTextoAsync_ChatDoesntExistTest()
        {
            // Arrange
            var mensajeRecibidoDTO = new MensajeTextoRecibidoDTO
            {
                ChatPlataformaId = "chat2",
                DateTime = DateTime.Now,
                Plataforma = "Test",
                Texto = "Hola",
                UsuarioId = "usuario"
            };

            var chatMock = new Mock<Chat>();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var chatRepositoryMock = new Mock<IChatRepository>();

            chatRepositoryMock.Setup(
                r => r.GetAsync(
                    mensajeRecibidoDTO.UsuarioId,
                    mensajeRecibidoDTO.ChatPlataformaId,
                    mensajeRecibidoDTO.Plataforma))
                .ThrowsAsync(new NotFoundException());

            chatRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Chat>()))
                .ReturnsAsync(chatMock.Object);

            var recibidorMensajes = new RecibidorMensajes(
                unitOfWorkMock.Object,
                chatRepositoryMock.Object);

            // Act
            await recibidorMensajes.RecibirMensajeTextoAsync(mensajeRecibidoDTO);

            // Assert
            chatMock.Verify(
                x => x.AñadirMensajeTextoRecibido(
                    mensajeRecibidoDTO.DateTime,
                    mensajeRecibidoDTO.Texto),
                Times.Once);

            chatRepositoryMock.Verify(
                r => r.InsertAsync(
                    It.Is<Chat>(
                        c => c.UsuarioId == mensajeRecibidoDTO.UsuarioId &&
                            c.ChatPlataformaId ==
                            mensajeRecibidoDTO.ChatPlataformaId &&
                            c.Plataforma == mensajeRecibidoDTO.Plataforma)));

            unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}