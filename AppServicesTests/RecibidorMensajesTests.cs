using Domain.Entities;

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

            var chat = new Chat
            {
                Id = 10,
                UsuarioId = "usuario",
                ChatPlataformaId = "chat2",
                Plataforma = "Test"
            };

            var mensaje = new MensajeTexto
            {
                ChatId = chat.Id,
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Usuario,
                Texto = "Hola",
            };

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var chatRepositoryMock = new Mock<IChatRepository>();
            var mensajeFactoryMock = new Mock<IMensajeFactory>();
            var mensajeRepositoryMock = new Mock<IMensajeRepository>();

            chatRepositoryMock.SetupSequence(
                r => r.GetAsync(
                    mensajeRecibidoDTO.UsuarioId,
                    mensajeRecibidoDTO.ChatPlataformaId,
                    mensajeRecibidoDTO.Plataforma))
                .ReturnsAsync(chat);

            mensajeFactoryMock.Setup(
                f => f.CreateMensajeTexto(
                    chat.Id,
                    mensajeRecibidoDTO.DateTime,
                    TipoMensaje.Usuario,
                    mensajeRecibidoDTO.Texto))
                .Returns(mensaje);

            var recibidorMensajes = new RecibidorMensajes(
                unitOfWorkMock.Object,
                chatRepositoryMock.Object,
                mensajeFactoryMock.Object,
                mensajeRepositoryMock.Object);

            // Act
            await recibidorMensajes.RecibirMensajeTextoAsync(mensajeRecibidoDTO);

            // Assert
            chatRepositoryMock.Verify(
                r => r.InsertAsync(It.IsAny<Chat>()),
                Times.Never);

            mensajeRepositoryMock.Verify(r => r.InsertAsync(mensaje), Times.Once);

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

            var chat = new Chat
            {
                UsuarioId = "usuario",
                ChatPlataformaId = "chat2",
                Plataforma = "Test"
            };

            var mensaje = new MensajeTexto
            {
                ChatId = 10,
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Usuario,
                Texto = "Hola",
            };

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var chatRepositoryMock = new Mock<IChatRepository>();
            var mensajeFactoryMock = new Mock<IMensajeFactory>();
            var mensajeRepositoryMock = new Mock<IMensajeRepository>();

            chatRepositoryMock.Setup(
                r => r.GetAsync(
                    mensajeRecibidoDTO.UsuarioId,
                    mensajeRecibidoDTO.ChatPlataformaId,
                    mensajeRecibidoDTO.Plataforma))
                .ThrowsAsync(new NotFoundException());

            chatRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Chat>()))
                .ReturnsAsync(chat);

            unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Callback(() => chat.Id = 10);

            mensajeFactoryMock.Setup(
                f => f.CreateMensajeTexto(
                    10,
                    mensajeRecibidoDTO.DateTime,
                    TipoMensaje.Usuario,
                    mensajeRecibidoDTO.Texto))
                .Returns(mensaje);

            var recibidorMensajes = new RecibidorMensajes(
                unitOfWorkMock.Object,
                chatRepositoryMock.Object,
                mensajeFactoryMock.Object,
                mensajeRepositoryMock.Object);

            // Act
            await recibidorMensajes.RecibirMensajeTextoAsync(mensajeRecibidoDTO);

            // Assert
            chatRepositoryMock.Verify(
                r => r.InsertAsync(
                    It.Is<Chat>(
                        c => c.UsuarioId == mensajeRecibidoDTO.UsuarioId &&
                            c.ChatPlataformaId == mensajeRecibidoDTO.ChatPlataformaId &&
                            c.Plataforma == mensajeRecibidoDTO.Plataforma)));

            mensajeRepositoryMock.Verify(r => r.InsertAsync(mensaje), Times.Once);

            unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
        }
    }
}