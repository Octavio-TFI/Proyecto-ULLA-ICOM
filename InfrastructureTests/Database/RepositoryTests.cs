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
    internal class RepositoryTests
    {
        [Test]
        public async Task GetAsync_Exists()
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            var repository = new MensajeRepository(context);

            var mensaje = new MensajeTexto
            {
                ChatId = 1,
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Usuario,
                Texto = "Hola"
            };

            await context.AddAsync(mensaje);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAsync(mensaje.Id);

            // Assert
            Assert.That(result, Is.EqualTo(mensaje));
        }

        [Test]
        public void GetAsync_DoesNotExist()
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            var repository = new MensajeRepository(context);

            // Act and Assert
            var result = Assert.ThrowsAsync<NotFoundException>(
                async () => await repository.GetAsync(1));
        }

        [Test]
        public async Task InsertAsync_ShouldInsertEntity()
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            var repository = new MensajeRepository(context);

            var mensaje = new MensajeTexto
            {
                ChatId = 1,
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Usuario,
                Texto = "Hola"
            };

            // Act
            var result = await repository.InsertAsync(mensaje);
            await context.SaveChangesAsync();

            // Assert
            Assert.That(result, Is.EqualTo(mensaje));
            Assert.That(context.Mensajes, Has.Member(result));
        }
    }
}
