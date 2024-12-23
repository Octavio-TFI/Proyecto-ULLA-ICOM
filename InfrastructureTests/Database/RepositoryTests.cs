﻿using Domain.Entities;
using Domain.ValueObjects;
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
        public async Task InsertAsync_ShouldInsertEntity()
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryContext();
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
