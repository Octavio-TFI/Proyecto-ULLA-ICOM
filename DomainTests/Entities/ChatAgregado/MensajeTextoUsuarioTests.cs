using Domain.Entities.ChatAgregado;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado.Tests
{
    internal class MensajeTextoUsuarioTests
    {
        [Test]
        public void ToString_WhenCalled_ReturnsTexto()
        {
            // Arrange
            var texto = "Hola, mundo!";
            var mensaje = new MensajeTextoUsuario
            {
                Texto = texto,
                DateTime = DateTime.Now
            };

            // Act
            var result = mensaje.ToString();

            // Assert
            Assert.AreEqual(texto, result);
        }
    }
}
