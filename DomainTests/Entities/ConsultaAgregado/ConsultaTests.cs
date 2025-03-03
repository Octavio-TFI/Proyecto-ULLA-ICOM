using Domain.Entities.ConsultaAgregado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ConsultaAgregado.Tests
{
    class ConsultaTests
    {
        [Test]
        public void ToStringTest()
        {
            // Arrange
            var consulta = new Consulta
            {
                RemoteId = 1,
                Titulo = "Titulo",
                Descripcion = "Descripcion",
                Solucion = "Solucion",
                EmbeddingTitulo = [1.0f, 2.0f],
                EmbeddingDescripcion = [3.0f, 4.0f]
            };

            var expected = @"# Titulo
## Descripcion
Descripcion
## Solucion
Solucion
";
            // Act
            var result = consulta.ToString();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
