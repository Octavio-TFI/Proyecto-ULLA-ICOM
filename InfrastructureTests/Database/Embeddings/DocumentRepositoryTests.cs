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
                    DocumentName = "fileName",
                    Texto = "Texto0",
                    Embedding = [0.1f, 0.2f, 0.3f],
                },
                new()
                {
                    DocumentName = "fileName",
                    Texto = "Texto1",
                    Embedding = [0.4f, 0.5f, 0.6f],
                },
                new()
                {
                    DocumentName = "fileName",
                    Texto = "Texto2",
                    Embedding = [0.7f, 0.8f, 0.9f],
                },
                new()
                {
                    DocumentName = "fileName",
                    Texto = "Texto3",
                    Embedding = [-0.7f, -0.8f, -0.9f],
                },
                new()
                {
                    DocumentName = "fileName",
                    Texto = "Texto4",
                    Embedding = [0.7f, 0.8f, 0.9f],
                },
                new()
                {
                    DocumentName = "fileName",
                    Texto = "Texto5",
                    Embedding = [0.7f, 0.8f, 0.9f],
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
            Assert.That(result, Has.Count.EqualTo(5));
            Assert.Multiple(
                () =>
                {
                    Assert.That(result[0].Id, Is.EqualTo(1));
                    Assert.That(result, Has.No.Member(documentos[3]));
                });
        }
    }
}
