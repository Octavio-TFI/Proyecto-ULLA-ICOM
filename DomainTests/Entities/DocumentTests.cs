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
                Childs =
                    [new Document
                    {
                        Filename = "file",
                        Texto = "Child 1",
                        Embedding = [4,5,6]
                    },
                        new Document
                    {
                        Filename = "file",
                        Texto = "Child 2",
                        Embedding = [7,8,9]
                    }]
            };

            document.Childs.First().Parent = document;
            document.Childs.Last().Parent = document;

            // Act
            var result = document.ToString();

            // Assert
            Assert.That(result, Is.EqualTo("Texto\r\nChild 1\r\nChild 2"));
        }
    }
}
