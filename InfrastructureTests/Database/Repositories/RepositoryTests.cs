using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using InfrastructureTests.Database.Tests;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Repositories.Tests
{
    internal class RepositoryTests
    {
        [Test]
        public async Task GetAsync_Exists()
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            var repository = new ChatRepository(context);

            var chat = new Chat
            {
                ChatPlataformaId = "1",
                UsuarioId = "2",
                Plataforma = "Test"
            };

            await context.AddAsync(chat);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAsync(chat.Id);

            // Assert
            Assert.That(result, Is.EqualTo(chat));
        }

        [Test]
        public void GetAsync_DoesNotExist()
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            var repository = new ChatRepository(context);

            // Act and Assert
            var result = Assert.ThrowsAsync<NotFoundException>(
                async () => await repository.GetAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task InsertAsync_ShouldInsertEntity()
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            var repository = new ChatRepository(context);

            var chat = new Chat
            {
                ChatPlataformaId = "1",
                UsuarioId = "2",
                Plataforma = "Test"
            };

            // Act
            var result = await repository.InsertAsync(chat);
            await context.SaveChangesAsync();

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(result, Is.EqualTo(chat));
                    Assert.That(context.Chats, Has.Member(result));
                });
        }
    }
}
