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
            Assert.Multiple(
                () =>
                {
                    Assert.That(result, Is.EqualTo(mensaje));
                    Assert.That(context.Mensajes, Has.Member(result));
                });
        }

        [Test]
        public async Task InsertRangeAsync_ShouldInsertEntities()
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            var repository = new MensajeRepository(context);
            var mensajes = new List<Mensaje>
            {
                new MensajeTexto
                {
                    ChatId = 1,
                    DateTime = DateTime.Now,
                    Tipo = TipoMensaje.Usuario,
                    Texto = "Hola"
                },
                new MensajeTexto
                {
                    ChatId = 1,
                    DateTime = DateTime.Now,
                    Tipo = TipoMensaje.Usuario,
                    Texto = "Chau"
                }
            };

            // Act
            var result = await repository.InsertRangeAsync(mensajes);
            await context.SaveChangesAsync();

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(result, Is.EqualTo(mensajes));
                    Assert.That(context.Mensajes, Has.Member(result[0]));
                    Assert.That(context.Mensajes, Has.Member(result[1]));
                });
        }
    }
}
