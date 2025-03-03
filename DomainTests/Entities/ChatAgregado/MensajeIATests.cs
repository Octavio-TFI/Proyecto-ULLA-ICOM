using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado.Tests
{
    class MensajeIATests
    {
        [Test]
        public void ToString_WhenCalled_ReturnsTexto()
        {
            // Arrange
            var texto = "Hola, mundo!";
            var mensaje = new MensajeIA
            {
                Texto = texto,
                DateTime = DateTime.Now
            };

            // Act
            var result = mensaje.ToString();

            // Assert
            Assert.That(result, Is.EqualTo(texto));
        }
    }
}
