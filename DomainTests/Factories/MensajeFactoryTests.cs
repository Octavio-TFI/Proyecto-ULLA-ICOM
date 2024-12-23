using Domain.Entities;
using Domain.Events;
using Domain.Factories;
using Domain.ValueObjects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Factories.Tests
{
    [TestFixture()]
    public class MensajeFactoryTests
    {
        [Test()]
        public void CreateMensajeTextoTest()
        {
            // Arrange
            int chatId = 5;
            DateTime dateTime = DateTime.Now;
            TipoMensaje tipoMensaje = TipoMensaje.Usuario;
            string texto = "Hola";

            var factory = new MensajeFactory();

            // Act
            var mensaje = factory.CreateMensajeTexto(
                chatId,
                dateTime,
                tipoMensaje,
                texto);

            // Assert
            Assert.That(mensaje.ChatId, Is.EqualTo(chatId));
            Assert.That(mensaje.DateTime, Is.EqualTo(dateTime));
            Assert.That(mensaje.Tipo, Is.EqualTo(tipoMensaje));
            Assert.That(mensaje.Texto, Is.EqualTo(texto));
            Assert.That(
                mensaje.Events,
                Has.One.TypeOf<MensajeRecibidoEvent>().With
                    .Property(nameof(MensajeRecibidoEvent.ChatId))
                    .EqualTo(mensaje.ChatId));
        }
    }
}