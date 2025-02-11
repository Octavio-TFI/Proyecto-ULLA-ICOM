using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Tests
{
    internal class DocumentTests
    {
        [Test]
        public void ToStringTest()
        {
            // Arrange
            var document = new Document
            {
                Filename = "file",
                Embedding = [1, 2, 3],
                Texto = "Texto",
            };

            // Act
            var result = document.ToString();

            // Assert
            Assert.That(result, Is.EqualTo("Texto"));
        }
    }
}
