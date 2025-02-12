using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.DocumentProcessing.Tests
{
    internal class MarkdownProcessorTests
    {
        const string MD = @"# Test 1

Texto del
Test 1

## Test 2

    Texto del Test 2

## Test 3
***
Texto del Test 3

### Test 4

| | Tabla |
| --- | --- |
| Columna
1 | Columna 2 |

### Ver también

### ver también

### Ver tambien

### Vea también

### vea también

### vea tambien

####    
";

        [Test]
        public async Task ProcessAsync_WithMarkdownDocument_ReturnsDocuments()
        {
            // Arrange
            var path = "path";
            var documentData = Encoding.UTF8.GetBytes(MD);

            var documentFactory = new Mock<IDocumentFactory>();

            documentFactory
                .Setup(
                    x => x.CreateAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<Document?>()))
                .ReturnsAsync(
                    (string filename, string texto, Document _) => new Document
                    {
                        Filename = filename,
                        Texto = texto,
                        Embedding = [1,2,3]
                    });

            var processor = new MarkdownProcessor(documentFactory.Object);

            // Act
            var documents = await processor.ProcessAsync(path, documentData);

            // Assert
            Assert.That(documents, Has.Count.EqualTo(4));
            Assert.That(
                documents,
                Has.All.With.Matches<Document>(d => d.Filename == "path"));
            Assert.That(
                documents,
                Has.All.With
                    .Matches<Document>(
                        d => d.Embedding.SequenceEqual([1, 2, 3])));

            Assert.That(
                documents[0].Texto,
                Is.EqualTo(
                    @"# Test 1

Texto del Test 1"));

            Assert.That(
                documents[1].Texto,
                Is.EqualTo(
                    @"# Test 1

## Test 2

Texto del Test 2"));

            Assert.That(
                documents[2].Texto,
                Is.EqualTo(
                    @"# Test 1

## Test 3

Texto del Test 3"));

            Assert.That(
                documents[3].Texto,
                Is.EqualTo(
                    @"# Test 1

## Test 3

### Test 4

" +
                        "| | Tabla |\n| --- | --- |\n| Columna 1 | Columna 2 |"));
        }
    }
}
