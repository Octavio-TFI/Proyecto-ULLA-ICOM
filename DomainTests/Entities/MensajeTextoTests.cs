using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Tests
{
    internal class MensajeTextoTests
    {
        [Test]
        public void ToString_WhenCalled_ReturnsTexto()
        {
            // Arrange
            var texto = "Hola, mundo!";
            var mensaje = new MensajeTexto
            {
                Texto = texto,
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Usuario
            };

            // Act
            var result = mensaje.ToString();

            // Assert
            Assert.AreEqual(texto, result);
        }
    }
}
