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
        // Prueba particionado de equivalencia:
        // Particion Valida: Mensajes con ID existentes
        [TestCase("1234", true, ExpectedResult = true)]
        [TestCase("1234", false, ExpectedResult = true)]
        // Particion Invalida: Mensajes con ID no existentes
        [TestCase("abcd", true, ExpectedResult = false)]
        [TestCase("abcd", false, ExpectedResult = false)]
        public async Task<bool> Calificacion_RecibeStatusCodeCorrecto(
            string idMensajeCalificacion,
            bool calificacion)
        {
            using var localLLMServer = WireMockServer.Start();
            using var chatServer = WireMockServer.Start();

            var apiFactory = await CreateAPIFactoryAsync(
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
                PlataformaMensajeId = "1234",
                DateTime = DateTime.Now
            };

            chat.Mensajes.Add(mensaje);

            await dbContext.AddAsync(chat);
            await dbContext.SaveChangesAsync();

            var calificacionMensaje = new TestCalificacionMensaje
            {
                MensajeId = idMensajeCalificacion,
                Calificacion = calificacion
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync(
                "/Test/calificacion",
                calificacionMensaje)
                .ConfigureAwait(false);

            // Assert
            return httpResponse.IsSuccessStatusCode;
        }

        // Prueba transicion de estado:
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task Calificacion_CambiaEstadoCalificacion(
            bool calificacion)
        {
            // Arrange
            using var localLLMServer = WireMockServer.Start();
            using var chatServer = WireMockServer.Start();

            var apiFactory = await CreateAPIFactoryAsync(
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
            var updatedDbContext = apiFactory.Services.CreateScope()
                .ServiceProvider
                .GetRequiredService<ChatContext>();

            var mensajeActualizado = await updatedDbContext.Set<MensajeIA>()
                .FindAsync(mensaje.Id);

            Assert.That(
                mensajeActualizado?.Calificacion,
                Is.EqualTo(calificacion));
        }
    }
}
