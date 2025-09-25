using Domain.Entities.DocumentoAgregado;
using InfrastructureTests.Database.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Repositories.Tests
{
    internal class DocumentRepositoryTests
    {
        [Test]
        public async Task DocumentsWithFilenameAsyncTest()
        {
            // Arrange
            var documentos = new List<Document>
            {
                new() { Filename = "fileName0", Texto = "Texto0" },
                new() { Filename = "fileName1", Texto = "Texto1" },
            };

            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
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
