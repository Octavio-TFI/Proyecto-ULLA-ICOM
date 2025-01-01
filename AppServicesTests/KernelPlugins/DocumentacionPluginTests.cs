using Microsoft.SemanticKernel.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.KernelPlugins.Tests
{
    internal class DocumentacionPluginTests
    {
        [Test]
        public async Task BuscarDocumentacionAsyncTest()
        {
            // Arrange
            var textEmbeddingGenerationService 
                = new Mock<ITextEmbeddingGenerationService>();

            var documentRepository = new Mock<IDocumentRepository>();
            var documentacionPlugin = new DocumentacionPlugin(
                textEmbeddingGenerationService.Object,
                documentRepository.Object);

            var consulta = "consulta";
            var embeddingConsulta = new ReadOnlyMemory<float>([1, 2, 3]);
            var document = new Document
            {
                Texto = "Documentacion",
                Embedding = [1,2,3]
            };
            var searchResult = new List<Document> { document };

            textEmbeddingGenerationService
                .Setup(
                    t => t.GenerateEmbeddingsAsync(
                        It.Is<IList<string>>(x => x.Contains(consulta)),
                        null,
                        default))
                .ReturnsAsync([embeddingConsulta]);

            documentRepository
                .Setup(d => d.GetDocumentosRelacionadosAsync(embeddingConsulta))
                .ReturnsAsync(searchResult);

            // Act
            var result = await documentacionPlugin.BuscarDocumentacionAsync(
                consulta);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(document.ToString(), result.First());
        }
    }
}
