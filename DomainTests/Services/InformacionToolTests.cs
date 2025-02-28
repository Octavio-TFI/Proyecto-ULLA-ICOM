using AppServices.Abstractions;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Tests
{
    internal class InformacionToolTests
    {
        [Test]
        public async Task BuscarInformacionAsyncTest()
        {
            // Arrange
            var embeddingServiceMock = new Mock<IEmbeddingService>();
            var consultaRepository = new Mock<IConsultaRepository>();
            var documentRepository = new Mock<IDocumentRepository>();
            var rankerMock = new Mock<IRanker>();

            var consulta = "consulta";
            var embeddingConsulta = new float[] { 1, 2, 3 };
            var consultaSimilar = new Consulta
            {
                RemoteId = 1,
                Titulo = "Consulta",
                Descripcion = "Descripcion",
                EmbeddingTitulo = [1, 2, 3],
                EmbeddingDescripcion = [4, 5, 6],
                Solucion = "Solucion",
            };

            var documentoRelacionado = new Document
            {
                Filename = "fileName",
                Texto = "Documento",
            };

            var consultas = new List<Consulta>
            {
                consultaSimilar,
                consultaSimilar
            };

            var documentos = new List<Document>
            {
                documentoRelacionado,
                documentoRelacionado
            };

            embeddingServiceMock
                .Setup(x => x.GenerateAsync(consulta))
                .ReturnsAsync(embeddingConsulta);

            consultaRepository
                .Setup(x => x.GetConsultasSimilaresAsync(embeddingConsulta))
                .ReturnsAsync(consultas);

            documentRepository.Setup(
                x => x.GetDocumentosRelacionadosAsync(embeddingConsulta))
                .ReturnsAsync(documentos);

            rankerMock.Setup(x => x.RankAsync(consultas, consulta))
                .ReturnsAsync([consultaSimilar]);

            rankerMock.Setup(x => x.RankAsync(documentos, consulta))
                .ReturnsAsync(documentos);

            var consultasPlugin = new InformacionTool(
                Mock.Of<ILogger<InformacionTool>>(),
                embeddingServiceMock.Object,
                consultaRepository.Object,
                documentRepository.Object,
                rankerMock.Object);

            // Act
            string result = await consultasPlugin
                .BuscarInformacionAsync(consulta)
                .ConfigureAwait(false);

            // Assert
            string expected = new StringBuilder()
                .Append("[Documentación]")
                .AppendLine()
                .Append(documentoRelacionado.ToString())
                .AppendLine()
                .Append(documentoRelacionado.ToString())
                .AppendLine()
                .Append("[Consultas Históricas]")
                .AppendLine()
                .Append(consultaSimilar.ToString())
                .ToString();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task BuscarInformacionAsync_NoDocumentsNoConsultasTest()
        {
            // Arrange
            var embeddingServiceMock
                = new Mock<IEmbeddingService>();
            var consultaRepository = new Mock<IConsultaRepository>();
            var documentRepository = new Mock<IDocumentRepository>();
            var rankerMock = new Mock<IRanker>();

            var consulta = "consulta";
            var embeddingConsulta = new float[] { 1, 2, 3 };

            embeddingServiceMock
                .Setup(x => x.GenerateAsync(consulta))
                .ReturnsAsync(embeddingConsulta);

            consultaRepository
                .Setup(x => x.GetConsultasSimilaresAsync(embeddingConsulta))
                .ReturnsAsync(new List<Consulta>());

            documentRepository.Setup(
                x => x.GetDocumentosRelacionadosAsync(embeddingConsulta))
                .ReturnsAsync(new List<Document>());

            rankerMock.Setup(
                x => x.RankAsync(It.IsAny<List<Consulta>>(), consulta))
                .ReturnsAsync(new List<Consulta>());

            rankerMock.Setup(
                x => x.RankAsync(It.IsAny<List<Document>>(), consulta))
                .ReturnsAsync(new List<Document>());

            var consultasPlugin = new InformacionTool(
                Mock.Of<ILogger<InformacionTool>>(),
                embeddingServiceMock.Object,
                consultaRepository.Object,
                documentRepository.Object,
                rankerMock.Object);

            // Act
            string result = await consultasPlugin
                .BuscarInformacionAsync(consulta)
                .ConfigureAwait(false);
            ;

            // Assert
            string expected = new StringBuilder()
                .Append("[Documentación]")
                .AppendLine()
                .AppendLine("No se encontro documentación relacionada")
                .AppendLine()
                .Append("[Consultas Históricas]")
                .AppendLine()
                .AppendLine("No se encontraron consultas históricas similares")
                .ToString();

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
