using Domain.Entities.ChatAgregado;
using Domain.Exceptions;
using InfrastructureTests.Database.Tests;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Database.Repositories.Tests
{
    internal class MensajeIARepositoryTests
    {
        [Test]
        public async Task GetAsyncPlataformaId_Found_ReturnsMensajeIA()
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            var repository = new MensajeIARepository(context);

            var chat = new Chat
            {
                UsuarioId = "1",
                ChatPlataformaId = "2",
                Plataforma = "Telegram"
            };

            var mensajeIA = new MensajeIA
            {
                Texto = "Hola",
                PlataformaMensajeId = "msg123",
                DateTime = DateTime.Now
            };

            chat.Mensajes.Add(mensajeIA);
            await context.AddAsync(chat);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAsync("msg123", "Telegram");

            // Assert
            Assert.That(result, Is.EqualTo(mensajeIA));
        }

        [Test]
        [TestCase("wrongId", "Telegram")]
        [TestCase("msg123", "WhatsApp")]
        [TestCase("wrongId", "WhatsApp")]
        public async Task GetAsyncPlataformaId_NotFound_ThrowsNotFoundException(
            string plataformaMensajeId,
            string plataforma)
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            var repository = new MensajeIARepository(context);

            var chat = new Chat
            {
                UsuarioId = "1",
                ChatPlataformaId = "2",
                Plataforma = "Telegram"
            };

            var mensajeIA = new MensajeIA
            {
                Texto = "Hola",
                PlataformaMensajeId = "msg123",
                DateTime = DateTime.Now
            };

            chat.Mensajes.Add(mensajeIA);
            await context.AddAsync(chat);
            await context.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(
                async () => await repository.GetAsync(
                    plataformaMensajeId,
                    plataforma));
        }

        [Test]
        public async Task GetAsync_NullPlataformaMensajeId_ThrowsNotFoundException(
            )
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            var repository = new MensajeIARepository(context);

            var chat = new Chat
            {
                UsuarioId = "1",
                ChatPlataformaId = "2",
                Plataforma = "Telegram"
            };

            var mensajeIA = new MensajeIA
            {
                Texto = "Hola",
                PlataformaMensajeId = null,  // Null ID
                DateTime = DateTime.Now
            };

            chat.Mensajes.Add(mensajeIA);
            await context.AddAsync(chat);
            await context.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(
                async () => await repository.GetAsync("msg123", "Telegram"));
        }
    }
}
