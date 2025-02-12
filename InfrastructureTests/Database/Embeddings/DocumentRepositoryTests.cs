using Domain.Entities;
using InfrastructureTests.Database.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Embeddings.Tests
{
    internal class DocumentRepositoryTests
    {
        [Test]
        public async Task GetDocumentosRelacionadosAsync()
        {
            // Arrange
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };

            var documentos = new List<Document>
            {
                new()
                {
                    Filename = "fileName",
                    Texto = "Texto0",
                    Embedding = [0.1f, 0.2f, 0.3f],
                },
                new()
                {
                    Filename = "fileName",
                    Texto = "Texto1",
                    Embedding = [0.4f, 0.5f, 0.6f],
                },
                new()
                {
                    Filename = "fileName",
                    Texto = "Texto2",
                    Embedding = [0.7f, 0.8f, 0.9f],
                },
                new()
                {
                    Filename = "fileName",
                    Texto = "Texto3",
                    Embedding = [-0.7f, -0.8f, -0.9f],
                },
                new()
                {
                    Filename = "fileName",
                    Texto = "Texto4",
                    Embedding = [0.7f, 0.8f, 0.9f],
                },
                new()
                {
                    Filename = "fileName",
                    Texto = "Texto5",
                    Embedding = [0.4f, 0.5f, 0.6f],
                },
                new()
                {
                    Filename = "fileName",
                    Texto = "Texto6",
                    Embedding = [0.1f, 0.2f, 0.3f],
                },
            };

            documentos.AddRange(
                Enumerable.Repeat(documentos.Last(), 20)
                    .Select(
                        x => new Document
                            {
                                Filename = x.Filename,
                                Texto = x.Texto,
                                Embedding = [24,4,5]
                            }));

            var context = DatabaseTestsHelper.CreateInMemoryEmbeddingContext();
            await context.Documents.AddRangeAsync(documentos);
            await context.SaveChangesAsync();

            var repository = new DocumentRepository(context);

            // Act
            var result = await repository.GetDocumentosRelacionadosAsync(
                embedding);

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(result, Has.Count.EqualTo(20));

                    Assert.That(result, Has.Member(documentos[0]));
                    Assert.That(result, Has.Member(documentos[1]));
                    Assert.That(result, Has.Member(documentos[2]));
                    Assert.That(result, Has.No.Member(documentos[3]));
                    Assert.That(result, Has.Member(documentos[4]));
                    Assert.That(result, Has.Member(documentos[5]));
                    Assert.That(result, Has.Member(documentos[6]));
                });
        }

        public async Task DocumentsWithFilenameAsyncTest()
        {
            // Arrange
            var documentos = new List<Document>
            {
                new()
                {
                    Filename = "fileName0",
                    Texto = "Texto0",
                    Embedding = [0.1f, 0.2f, 0.3f],
                },
                new()
                {
                    Filename = "fileName1",
                    Texto = "Texto1",
                    Embedding = [0.4f, 0.5f, 0.6f],
                },
            };

            var context = DatabaseTestsHelper.CreateInMemoryEmbeddingContext();
            await context.Documents.AddRangeAsync(documentos);
            await context.SaveChangesAsync();

            var repository = new DocumentRepository(context);

            // Act
            var filenames = await repository.GetAllFilenamesAsync();


            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(filenames, Has.Count.EqualTo(2));
                    Assert.That(filenames, Has.Member("fileName0"));
                    Assert.That(filenames, Has.Member("fileName1"));
                });
        }
    }
}
