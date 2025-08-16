using Domain.Abstractions;
using Domain.Events;
using Domain.ValueObjects;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado.Tests
{
    class ChatTests
    {
        [Test]
        public void AñadirMensajeTextoRecibidoTest()
        {
            // Arrange
            var chat = new Chat
            {
                ChatPlataformaId = "1",
                Plataforma = "WhatsApp",
                UsuarioId = "1"
            };

            var dateTime = DateTime.Now;
            var texto = "Hola";

            // Act
            var mensaje = chat.AñadirMensajeTextoRecibido(dateTime, texto);

            // Assert
            Assert.That(chat.Mensajes.Count, Is.EqualTo(1));
            Assert.That(
                chat.Mensajes.Last(),
                Is.TypeOf<MensajeTextoUsuario>().With
                    .Property(nameof(MensajeTextoUsuario.Texto))
                    .EqualTo(texto)
                    .And
                    .Property(nameof(MensajeTextoUsuario.DateTime))
                    .EqualTo(dateTime));
            Assert.That(
                chat.Events,
                Has.One.With
                    .Property(nameof(MensajeRecibidoEvent.EntityId))
                    .EqualTo(chat.Id));
        }

        [Test]
        public async Task GenerarMensajeAsyncTest()
        {
            // Arrange
            var chat = new Chat
            {
                ChatPlataformaId = "1",
                Plataforma = "WhatsApp",
                UsuarioId = "1"
            };

            chat.Mensajes
                .Add(
                    new MensajeTextoUsuario
                    {
                        Texto = "Hola",
                        DateTime = DateTime.Now
                    });

            var agente = new Mock<IAgent>();

            var agentResult = new AgentResult
            {
                Texto = "Respuesta del agente",
                FunctionCalls = []
            };

            agente.Setup(a => a.GenerarRespuestaAsync(chat.Mensajes, null))
                .ReturnsAsync(agentResult);

            // Act
            var mensaje = await chat.GenerarMensajeAsync(agente.Object)
                .ConfigureAwait(false);

            // Assert
            Assert.That(chat.Mensajes.Count, Is.EqualTo(2));
            Assert.That(
                chat.Mensajes.Last(),
                Is.TypeOf<MensajeIA>().With
                    .Property(nameof(MensajeIA.Texto))
                    .EqualTo(agentResult.Texto)
                    .And
                    .Property(nameof(MensajeIA.DateTime))
                    .Property(nameof(DateTime.Date))
                    .EqualTo(DateTime.Now.Date));
            Assert.That(
                chat.Events,
                Has.One.With
                    .Property(nameof(MensajeIAGeneradoEvent.EntityId))
                    .EqualTo(chat.Id));
        }

        [Test]
        public async Task GenerarMensajeAsync_ConFunctionCall_CreaMessageLlamadaHerramienta(
            )
        {
            // Arrange
            var chat = new Chat
            {
                ChatPlataformaId = "1",
                Plataforma = "WhatsApp",
                UsuarioId = "1"
            };

            chat.Mensajes
                .Add(
                    new MensajeTextoUsuario
                    {
                        Texto = "¿Puedes buscar información sobre el clima?",
                        DateTime = DateTime.Now
                    });

            var agente = new Mock<IAgent>();

            var functionCall = new AgentFunctionCall
            {
                PluginName = "WeatherPlugin",
                FunctionName = "GetWeather",
                Arguments =
                    new Dictionary<string, object?>
                    {
                        ["location"] = "Madrid",
                        ["units"] = "metric"
                    }
            };

            var agentResult = new AgentResult
            {
                Texto = string.Empty,
                FunctionCalls = [functionCall]
            };

            agente.Setup(a => a.GenerarRespuestaAsync(chat.Mensajes, null))
                .ReturnsAsync(agentResult);

            // Act
            var mensaje = await chat.GenerarMensajeAsync(agente.Object)
                .ConfigureAwait(false);

            // Assert
            Assert.That(chat.Mensajes.Count, Is.EqualTo(2));

            var llamadaHerramienta = chat.Mensajes.Last();
            Assert.That(
                llamadaHerramienta,
                Is.TypeOf<MensajeLlamadaHerramienta>());

            Assert.Multiple(
                () =>
                {
                    Assert.That(
                        llamadaHerramienta,
                        Has.Property(
                                nameof(MensajeLlamadaHerramienta.PluginName))
                                .EqualTo("WeatherPlugin"));
                    Assert.That(
                        llamadaHerramienta,
                        Has.Property(
                                nameof(MensajeLlamadaHerramienta.FunctionName))
                                .EqualTo("GetWeather"));
                    Assert.That(
                        llamadaHerramienta,
                        Has.Property(
                                nameof(MensajeLlamadaHerramienta.Argumentos))
                                .Not.Null);
                    Assert.That(
                        llamadaHerramienta,
                        Has.Property(nameof(MensajeLlamadaHerramienta.DateTime))
                                .Property(nameof(DateTime.Date))
                                .EqualTo(DateTime.Now.Date));
                });

            var argumentos = ((MensajeLlamadaHerramienta)llamadaHerramienta).Argumentos!;
            Assert.Multiple(
                () =>
                {
                    Assert.That(
                        argumentos["location"]!.ToString(),
                        Is.EqualTo("Madrid"));
                    Assert.That(
                        argumentos["units"]!.ToString(),
                        Is.EqualTo("metric"));
                });

            Assert.That(
                chat.Events,
                Has.One.With
                    .Property(nameof(LlamadaHerramientaGeneradaEvent.EntityId))
                    .EqualTo(chat.Id)
                    .And
                    .Property(
                        nameof(
                                LlamadaHerramientaGeneradaEvent.MensajeLlamadaHerramientaGeneradaId))
                    .EqualTo(mensaje.Id));
        }

        // TODO: Add tests for LlamarHerramientaAsync
    }
}
