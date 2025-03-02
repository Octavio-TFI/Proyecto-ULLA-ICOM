using Domain.Entities.DocumentoAgregado;
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
                    Filename = "fileName0",
                    Texto = "Texto0",
                    Chunks =
                        [new()
                        {
                            Texto = "Chunck1",
                            Embedding = [0.1f, 0.2f, 0.3f]
                        },new()
                        {
                            Texto = "Chunck2",
                            Embedding = [0.4f, 0.5f, 0.6f]
                        }],
                },
                new()
                {
                    Filename = "fileName1",
                    Texto = "Texto1",
                    Chunks =
                        [new()
                        {
                            Texto = "Chunck3",
                            Embedding = [-0.7f, -0.8f, -0.9f],
                        }, new()
                        {
                            Texto = "Chunck4",
                            Embedding = [0.1f, 0.2f, 0.3f],
                        }],
                },
                new()
                {
                    Filename = "fileName2",
                    Texto = "Texto2",
                    Chunks =
                        [new()
                        {
                            Texto = "Chunck5",
                            Embedding = [-0.7f, -0.8f, -0.9f],
                        }]
                },
                new()
                {
                    Filename = "fileName3",
                    Texto = "Texto3",
                    Chunks =
                        [new()
                        {
                            Texto = "Chunck6",
                            Embedding = [0.7f, 0.8f, 0.9f],
                        }]
                },
                new()
                {
                    Filename = "fileName4",
                    Texto = "Texto4",
                    Chunks =
                        [new()
                        {
                            Texto = "Chunck7",
                            Embedding = [0.4f, 0.5f, 0.6f],
                        }]
                },
                new()
                {
                    Filename = "fileName5",
                    Texto = "Texto5",
                    Chunks =
                        [new()
                        {
                            Texto = "Chunck8",
                            Embedding = [0.1f, 0.2f, 0.3f]
                        }]
                },
            };

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
                    Assert.That(result, Has.Count.EqualTo(5));

                    Assert.That(result, Has.Member(documentos[0]));
                    Assert.That(result, Has.Member(documentos[1]));
                    Assert.That(result, Has.No.Member(documentos[2]));
                    Assert.That(result, Has.Member(documentos[3]));
                    Assert.That(result, Has.Member(documentos[4]));
                    Assert.That(result, Has.Member(documentos[5]));
                });
        }

        [Test]
        public async Task DocumentsWithFilenameAsyncTest()
        {
            // Arrange
            var documentos = new List<Document>
            {
                new() { Filename = "fileName0", Texto = "Texto0" },
                new() { Filename = "fileName1", Texto = "Texto1" },
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
