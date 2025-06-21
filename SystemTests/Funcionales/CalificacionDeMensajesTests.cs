using AppServices.Abstractions;
using Controllers.DTOs;
using Domain.Entities.ChatAgregado;
using Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Tests;
using System.Text;
using System.Threading.Tasks;
using WireMock.Server;

namespace System.Tests.Funcionales
{
    public class CalificacionDeMensajesTests
        : BaseTests
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task Calificacion_MensajeExisteTest(bool calificacion)
        {
            // Arrange
            using var localLLMServer = WireMockServer.Start();
            using var chatServer = WireMockServer.Start();

            var apiFactory = CreateAPIFactory(
                localLLMServer.Port,
                chatServer.Port);

            var client = apiFactory.CreateClient();

            var dbContext = apiFactory.Services.CreateScope().ServiceProvider
                .GetRequiredService<ChatContext>();

            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                ChatPlataformaId = Guid.NewGuid().ToString(),
                UsuarioId = Guid.NewGuid().ToString(),
                Plataforma = Platforms.Test
            };

            var mensaje = new MensajeIA
            {
                Id = Guid.NewGuid(),
                Texto = "Hola, ¿cómo estás?",
                PlataformaMensajeId = Guid.NewGuid().ToString(),
                DateTime = DateTime.Now
            };

            chat.Mensajes.Add(mensaje);

            await dbContext.AddAsync(chat);
            await dbContext.SaveChangesAsync();

            var calificacionMensaje = new TestCalificacionMensaje
            {
                MensajeId = mensaje.PlataformaMensajeId,
                Calificacion = calificacion
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync(
                "/Test/calificacion",
                calificacionMensaje)
                .ConfigureAwait(false);

            // Assert
            Assert.That(httpResponse.IsSuccessStatusCode);

            var updatedDbContext = apiFactory.Services.CreateScope()
                .ServiceProvider
                .GetRequiredService<ChatContext>();

            var mensajeActualizado = await updatedDbContext.Set<MensajeIA>()
                .FindAsync(mensaje.Id);

            Assert.That(
                mensajeActualizado?.Calificacion,
                Is.EqualTo(calificacion));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task Calificacion_MensajeNoExisteTest(bool calificacion)
        {
            // Arrange
            using var localLLMServer = WireMockServer.Start();
            using var chatServer = WireMockServer.Start();

            var apiFactory = CreateAPIFactory(
                localLLMServer.Port,
                chatServer.Port);

            var client = apiFactory.CreateClient();

            var calificacionMensaje = new TestCalificacionMensaje
            {
                MensajeId = "id",
                Calificacion = calificacion
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync(
                "/Test/calificacion",
                calificacionMensaje)
                .ConfigureAwait(false);

            // Assert
            Assert.That(httpResponse.IsSuccessStatusCode, Is.False);
        }
    }
}
