using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using Infrastructure.Database.Chats;
using InfrastructureTests.Database.Tests;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Tests
{
    internal class ChatRepositoryTests
    {
        [Test]
        public async Task GetAsync_Found_ReturnsChat()
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            var repository = new ChatRepository(context);

            var chat = new Chat
            {
                Id = 1,
                UsuarioId = "1",
                ChatPlataformaId = "2",
                Plataforma = "Telegram"
            };

            await context.AddAsync(chat);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAsync("1", "2", "Telegram");

            // Assert
            Assert.That(result, Is.EqualTo(chat));
        }

        [Test]
        [TestCase("3", "2", "Telegram")]
        [TestCase("1", "3", "Telegram")]
        [TestCase("1", "2", "WhatsApp")]
        [TestCase("3", "3", "Telegram")]
        [TestCase("1", "3", "WhatsApp")]
        [TestCase("3", "2", "WhatsApp")]
        [TestCase("3", "3", "WhatsApp")]
        public async Task GetAsync_NotFound_ThrowsNotFoundException(
            string usuarioId,
            string chatPlataformaId,
            string plataforma)
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            var repository = new ChatRepository(context);

            var chat = new Chat
            {
                UsuarioId = usuarioId,
                ChatPlataformaId = chatPlataformaId,
                Plataforma = plataforma
            };

            await context.AddAsync(chat);
            await context.SaveChangesAsync();

            // Act
            Assert.ThrowsAsync<NotFoundException>(
                async () => await repository.GetAsync("1", "2", "Telegram"));
        }
    }
}
