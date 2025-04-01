using Domain.Abstractions;
using Domain.Events;
using Domain.ValueObjects;
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
                    .Matches<MensajeTextoUsuario>(m => m.Texto == texto)
                    .And
                    .Matches<MensajeTextoUsuario>(m => m.DateTime == dateTime));
            Assert.That(
                chat.Events,
                Has.One.With
                    .Matches<MensajeRecibidoEvent>(e => e.EntityId == chat.Id));
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
            var agentData = new AgentData();

            List<DocumentoRecuperado> documentosRecuperados = [
                new DocumentoRecuperado
            {
                DocumentoId = Guid.NewGuid(),
                Rank = true
            },
                new DocumentoRecuperado
            {
                DocumentoId = Guid.NewGuid(),
                Rank = false
            }];

            agentData.InformacionRecuperada.AddRange(documentosRecuperados);

            List<ConsultaRecuperada> consultasRecuperadas = [
                new ConsultaRecuperada
            {
                ConsultaId = Guid.NewGuid(),
                Rank = true
            },
                new ConsultaRecuperada
            {
                ConsultaId = Guid.NewGuid(),
                Rank = false
            }];

            agentData.InformacionRecuperada.AddRange(consultasRecuperadas);

            var agentResult = new AgentResult
            {
                Texto = string.Empty,
                AgentData = agentData
            };

            agente.Setup(
                a => a.GenerarRespuestaAsync(chat.Mensajes, null))
                .ReturnsAsync(agentResult);

            // Act
            var mensaje = await chat.GenerarMensajeAsync(agente.Object)
                .ConfigureAwait(false);

            // Assert
            Assert.That(chat.Mensajes.Count, Is.EqualTo(2));
            Assert.That(
                chat.Mensajes.Last(),
                Is.TypeOf<MensajeIA>().With
                    .Matches<MensajeIA>(m => m.Texto == agentResult.Texto)
                    .And
                    .Matches<MensajeIA>(m => m.DateTime.Date == DateTime.Now.Date)
                    .And
                    .Matches<MensajeIA>(
                        m => m.DocumentosRecuperados
                                .SequenceEqual(documentosRecuperados))
                    .And
                    .Matches<MensajeIA>(
                        m => m.ConsultasRecuperadas
                                .SequenceEqual(consultasRecuperadas)));
        }
    }
}
