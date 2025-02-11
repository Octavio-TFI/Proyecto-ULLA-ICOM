using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Factories.Tests
{
    internal class DocumentFactoryTests
    {
        [Test]
        public async Task CreateAsync_WithParent()
        {
            // Arrange
            var filename = "filename";
            var text = "text";
            var parent = new Document
            {
                Texto = "Parent",
                Embedding = [4,5,6],
                Filename = "Filename"
            };
            var embedding = new float[] { 1, 2, 3 };
            var embeddingGenerator = new Mock<ITextEmbeddingGenerationService>();

            embeddingGenerator
                .Setup(
                    x => x.GenerateEmbeddingsAsync(
                        It.Is<IList<string>>(x => x.Contains(text)),
                        null,
                        default))
                .ReturnsAsync([new ReadOnlyMemory<float>(embedding)]);

            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton(embeddingGenerator.Object);

            var factory = new DocumentFactory(kernelBuilder.Build());

            // Act
            var result = await factory.CreateAsync(filename, text, parent);

            // Assert
            Assert.That(filename, Is.EqualTo(result.Filename));
            Assert.That(text, Is.EqualTo(result.Texto));
            Assert.That(embedding, Is.EqualTo(result.Embedding));
        }

        [Test]
        public async Task CreateAsync_WithoutParent()
        {
            // Arrange
            var filename = "filename";
            var text = "text";
            var embedding = new float[] { 1, 2, 3 };
            var embeddingGenerator = new Mock<ITextEmbeddingGenerationService>();

            embeddingGenerator
                .Setup(
                    x => x.GenerateEmbeddingsAsync(
                        It.Is<IList<string>>(x => x.Contains(text)),
                        null,
                        default))
                .ReturnsAsync([new ReadOnlyMemory<float>(embedding)]);

            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton(embeddingGenerator.Object);

            var factory = new DocumentFactory(kernelBuilder.Build());

            // Act
            var result = await factory.CreateAsync(filename, text, null);

            // Assert
            Assert.That(filename, Is.EqualTo(result.Filename));
            Assert.That(text, Is.EqualTo(result.Texto));
            Assert.That(embedding, Is.EqualTo(result.Embedding));
        }
    }
}
