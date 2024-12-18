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
        public async Task RecibirMensajeAsyncTest()
        {
            // Arrange
            var mensaje = new MensajeTextoDTO
            {
                ChatId = "chat2",
                DateTime = DateTime.Now,
                Plataforma = "Test",
                Texto = "Hola",
                UsuarioId = "usuario"
            };

            var chat = new Chat
            {
                Id = 10,
                UsuarioId = "usuario",
                ChatId = "chat2",
                Plataforma = "Test"
            };

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var chatRepositoryMock = new Mock<IChatRepository>();
            var mensajeRepositoryMock = new Mock<IMensajeRepository>();

            chatRepositoryMock.Setup(
                r => r.GetAsync(
                    mensaje.UsuarioId,
                    mensaje.ChatId,
                    mensaje.Plataforma))
                .ReturnsAsync(chat);

            unitOfWorkMock.Setup(u => u.Chats)
                .Returns(chatRepositoryMock.Object);

            unitOfWorkMock.Setup(u => u.Mensajes)
                .Returns(mensajeRepositoryMock.Object);

            var recibidorMensajes = new RecibidorMensajes(unitOfWorkMock.Object);

            // Act
            await recibidorMensajes.RecibirMensajeTextoAsync(mensaje);

            // Assert
            mensajeRepositoryMock.Verify(
                r => r.SaveAsync(It.Is<MensajeTexto>(m => m.ChatId == chat.Id)),
                Times.Once);
        }
    }
}