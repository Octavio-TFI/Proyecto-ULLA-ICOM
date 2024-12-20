using AppServices;
using AppServices.Abstractions.DTOs;
using AppServices.Ports;
using Domain.Entities;
using Domain.Exceptions;
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
        public async Task RecibirMensajeTextoAsync_ChatExistsTest()
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
                .ReturnsAsync(chat);

            var recibidorMensajes = new RecibidorMensajes(
                unitOfWorkMock.Object,
                chatRepositoryMock.Object,
                mensajeRepositoryMock.Object);

            // Act
            await recibidorMensajes.RecibirMensajeTextoAsync(mensajeDTO);

            // Assert
            chatRepositoryMock.Verify(
                r => r.InsertAsync(It.IsAny<Chat>()),
                Times.Never);

            mensajeRepositoryMock.Verify(
                r => r.InsertAsync(
                    It.Is<MensajeTexto>(
                        m => m.ChatId == chat.Id &&
                            m.DateTime == mensajeDTO.DateTime &&
                            m.Texto == mensajeDTO.Texto)),
                Times.Once);

            unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task RecibirMensajeTextoAsync_ChatDoesntExistTest()
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
                .ThrowsAsync(new NotFoundException())
                .ReturnsAsync(chat);

            unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .Callback(() => chat.Id = 10);

            var recibidorMensajes = new RecibidorMensajes(
                unitOfWorkMock.Object,
                chatRepositoryMock.Object,
                mensajeRepositoryMock.Object);

            // Act
            await recibidorMensajes.RecibirMensajeTextoAsync(mensajeDTO);

            // Assert
            chatRepositoryMock.Verify(
                r => r.InsertAsync(
                    It.Is<Chat>(
                        c => c.UsuarioId == mensajeDTO.UsuarioId &&
                            c.ChatPlataformaId == mensajeDTO.ChatPlataformaId &&
                            c.Plataforma == mensajeDTO.Plataforma)));

            mensajeRepositoryMock.Verify(
                r => r.InsertAsync(
                    It.Is<MensajeTexto>(
                        m => m.ChatId == 10 &&
                            m.DateTime == mensajeDTO.DateTime &&
                            m.Texto == mensajeDTO.Texto)),
                Times.Once);

            unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
        }
    }
}