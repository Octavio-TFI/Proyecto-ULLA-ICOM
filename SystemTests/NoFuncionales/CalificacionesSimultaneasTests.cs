using AppServices.Abstractions;
using Controllers.DTOs;
using Domain.Entities.ChatAgregado;
using Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WireMock.Server;

namespace System.Tests.NoFuncionales
{
    public class CalificacionesSimultaneasTests
        : BaseTests
    {
        [Test]
        public async Task CalificacionesSimultaneas50Test()
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

            var mensajes = Enumerable.Range(0, 50)
                .Select(
                    i => new MensajeIA
                    {
                        Id = Guid.NewGuid(),
                        Texto = $"Mensaje {i + 1}",
                        PlataformaMensajeId = Guid.NewGuid().ToString(),
                        DateTime = DateTime.Now.AddSeconds(i)
                    })
                .ToList();

            chat.Mensajes.AddRange(mensajes);

            await dbContext.AddAsync(chat);
            await dbContext.SaveChangesAsync();

            var calificacionesMensajes = mensajes.Select(
                m => new TestCalificacionMensaje
                {
                    MensajeId = m.PlataformaMensajeId!,
                    Calificacion = true
                })
                .ToList();

            // Act
            var responsesTasks = calificacionesMensajes.Select(
                calificacionMensaje => client.PostAsJsonAsync(
                    "/Test/calificacion",
                    calificacionMensaje))
                .ToList();

            var responses = await Task.WhenAll(responsesTasks);

            // Assert
            Assert.That(
                responses.Select(x => x.IsSuccessStatusCode),
                Is.All.True,
                "Todos los requests deben ser exitosos");

            var updatedDbContext = apiFactory.Services.CreateScope()
                .ServiceProvider
                .GetRequiredService<ChatContext>();

            Assert.That(
                updatedDbContext.MensajesIA.Select(x => x.Calificacion),
                Is.All.True,
                "Todas las calificaciones deben ser true.");

            Assert.That(
                updatedDbContext.MensajesIA.Select(x => x.Calificacion).Count(),
                Is.EqualTo(50),
                "Debe haber 50 calificaciones.");
        }
    }
}
