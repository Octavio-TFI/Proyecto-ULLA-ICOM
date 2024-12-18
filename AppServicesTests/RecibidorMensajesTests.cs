using AppServices;
using AppServices.Abstractions.DTOs;
using Domain.Entities;
using Domain.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Tests
{
    [TestFixture()]
    public class RecibidorMensajesTests
    {
        [Test()]
        [TestCase(false)]
        [TestCase(true)]
        public async Task RecibirMensajeTextoAsyncTest(bool chatExists)
        {
            // Arrange
            var mensajeDTO = new MensajeTextoDTO
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

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var chatRepositoryMock = new Mock<IChatRepository>();
            var mensajeRepositoryMock = new Mock<IMensajeRepository>();

            chatRepositoryMock.SetupSequence(
                r => r.GetAsync(
                    mensajeDTO.UsuarioId,
                    mensajeDTO.ChatPlataformaId,
                    mensajeDTO.Plataforma))
                .ReturnsAsync(chatExists ? chat : null);

            chatRepositoryMock.Setup(
                r => r.InsertAsync(
                    It.Is<Chat>(
                        c => c.UsuarioId == mensajeDTO.UsuarioId &&
                            c.ChatPlataformaId == mensajeDTO.ChatPlataformaId &&
                            c.Plataforma == mensajeDTO.Plataforma)))
                .ReturnsAsync(chat);

            unitOfWorkMock.Setup(u => u.Chats)
                .Returns(chatRepositoryMock.Object);

            unitOfWorkMock.Setup(u => u.Mensajes)
                .Returns(mensajeRepositoryMock.Object);

            var recibidorMensajes = new RecibidorMensajes(unitOfWorkMock.Object);

            // Act
            await recibidorMensajes.RecibirMensajeTextoAsync(mensajeDTO);

            // Assert
            chatRepositoryMock.Verify(
                r => r.InsertAsync(It.IsAny<Chat>()),
                chatExists ? Times.Never : Times.Once);

            mensajeRepositoryMock.Verify(
                r => r.InsertAsync(
                    It.Is<MensajeTexto>(
                        m => m.ChatId == chat.Id &&
                            m.DateTime == mensajeDTO.DateTime &&
                            m.Texto == mensajeDTO.Texto)),
                Times.Once);

            unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}