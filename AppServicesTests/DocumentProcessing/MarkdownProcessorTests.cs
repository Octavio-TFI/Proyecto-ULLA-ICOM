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

#### Test 4

| | Tabla |
| --- | --- |
| Columna
1 | Columna 2 |

####    

## Test 5
asd
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
                        It.IsAny<IList<string>>()))
                .ReturnsAsync(
                    (string filename, string texto, IEnumerable<string> chunks) =>
                    {
                        var doc = new Document
                        {
                            Filename = filename,
                            Texto = texto,
                            Chunks = []
                        };

                        doc.Chunks = chunks.Select(
                            c => new DocumentChunk
                            {
                                Texto = c,
                                Embedding = [1,2,3]
                            })
                            .ToList();

                        return doc;
                    });

            var processor = new MarkdownProcessor(documentFactory.Object);

            // Act
            var documents = await processor.ProcessAsync(path, documentData);

            // Assert
            Assert.That(documents.Filename, Is.EqualTo("path"));
            Assert.That(documents.Chunks, Has.Count.EqualTo(5));
            Assert.That(
                documents.Chunks,
                Has.All.With
                    .Matches<DocumentChunk>(
                        d => d.Embedding.SequenceEqual([1, 2, 3])));

            Assert.That(
                documents.Chunks.First().Texto,
                Is.EqualTo(
                    @"# Test 1

Texto del Test 1"));

            Assert.That(
                documents.Chunks.Skip(1).First().Texto,
                Is.EqualTo(
                    @"# Test 1

## Test 2

Texto del Test 2"));

            Assert.That(
                documents.Chunks.Skip(2).First().Texto,
                Is.EqualTo(
                    @"# Test 1

## Test 3

Texto del Test 3"));

            Assert.That(
                documents.Chunks.Skip(3).First().Texto,
                Is.EqualTo(
                    @"# Test 1

## Test 3

#### Test 4

" +
                        "| | Tabla |\n| --- | --- |\n| Columna 1 | Columna 2 |"));

            Assert.That(
                documents.Chunks.Skip(4).First().Texto,
                Is.EqualTo(
                    @"# Test 1

## Test 5

asd"));
        }
    }
}
