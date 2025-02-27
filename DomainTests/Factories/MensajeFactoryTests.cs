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
        public void CreateMensajeTextoGeneradoTest()
        {
            // Arrange
            int chatId = 5;
            string texto = "Hola";
            var dateTime = DateTime.Now;

            var factory = new MensajeFactory();

            // Act
            var mensaje = factory.CreateMensajeTextoGenerado(chatId, texto);

            // Assert
            Assert.That(mensaje.ChatId, Is.EqualTo(chatId));
            Assert.That(mensaje.DateTime, Is.GreaterThan(dateTime));
            Assert.That(mensaje.Tipo, Is.EqualTo(TipoMensaje.Asistente));
            Assert.That(mensaje.Texto, Is.EqualTo(texto));
            Assert.That(
                mensaje.Events,
                Has.One.TypeOf<MensajeGeneradoEvent>().With
                    .Property(nameof(MensajeGeneradoEvent.Entity))
                    .EqualTo(mensaje));
        }
    }
}