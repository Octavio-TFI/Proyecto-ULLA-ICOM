using Microsoft.SemanticKernel.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.KernelPlugins.Tests
{
    internal class ConsultasPluginTests
    {
        [Test]
        public async Task BuscarConsultasAsyncTest()
        {
            // Arrange
            var textEmbeddingGenerationService
                = new Mock<ITextEmbeddingGenerationService>();
            var consultaRepository = new Mock<IConsultaRepository>();
            var rankerMock = new Mock<IRanker>();

            var consultasPlugin = new ConsultasPlugin(
                textEmbeddingGenerationService.Object,
                consultaRepository.Object,
                rankerMock.Object);

            var consulta = "consulta";
            var embeddingConsulta = new ReadOnlyMemory<float>([1, 2, 3]);
            var consultaSimilar = new Consulta
            {
                Titulo = "Consulta",
                Descripcion = "Descripcion",
                EmbeddingTitulo = [1, 2, 3],
                EmbeddingDescripcion = [4, 5, 6],
                Solucion = "Solucion",
            };

            var searchResult = new List<Consulta>
            {
                consultaSimilar,
                consultaSimilar
            };

            textEmbeddingGenerationService
                .Setup(
                    t => t.GenerateEmbeddingsAsync(
                        It.Is<IList<string>>(x => x.Contains(consulta)),
                        null,
                        default))
                .ReturnsAsync([embeddingConsulta]);

            consultaRepository
                .Setup(d => d.GetConsultasSimilaresAsync(embeddingConsulta))
                .ReturnsAsync(searchResult);

            rankerMock.Setup(x => x.RankAsync(searchResult, consulta))
                .ReturnsAsync([consultaSimilar]);

            // Act
            var result = await consultasPlugin.BuscarConsultasAsync(consulta);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(consultaSimilar.ToString(), Is.EqualTo(result.First()));
        }
    }
}
