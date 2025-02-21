using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Factories.Tests
{
    internal class DocumentFactoryTests
    {
        [Test]
        public async Task CreateAsyncTest()
        {
            // Arrange
            var filename = "testfile";
            var text = "sample text";
            List<string> textChunks = ["sample1", "sample2"];
            List<float[]> embeddings = [[1, 2, 3],[4, 5, 6],];

            var embeddingGeneratorMock = new Mock<ITextEmbeddingGenerationService>(
                );

            embeddingGeneratorMock
                .Setup(
                    x => x.GenerateEmbeddingsAsync(textChunks, null, default))
                .ReturnsAsync(
                    [new ReadOnlyMemory<float>(embeddings[0]),
                    new ReadOnlyMemory<float>(embeddings[1])]);

            var factory = new DocumentFactory(embeddingGeneratorMock.Object);

            // Act
            var result = await factory.CreateAsync(filename, text, textChunks);

            // Assert
            Assert.That(result.Filename, Is.EqualTo(filename));
            Assert.That(result.Texto, Is.EqualTo(text));
            Assert.That(result.Chunks, Has.Count.EqualTo(2));
            Assert.That(
                result.Chunks,
                Has.One.With.Matches<DocumentChunk>(c => c.Texto == "sample1"));
            Assert.That(
                result.Chunks,
                Has.One.With.Matches<DocumentChunk>(c => c.Texto == "sample2"));
            Assert.That(
                result.Chunks,
                Has.One.With
                    .Matches<DocumentChunk>(
                        c => c.Embedding.SequenceEqual(embeddings[0])));
            Assert.That(
                result.Chunks,
                Has.One.With
                    .Matches<DocumentChunk>(
                        c => c.Embedding.SequenceEqual(embeddings[1])));
        }
    }
}
