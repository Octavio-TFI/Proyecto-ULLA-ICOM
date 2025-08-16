using AppServices.Abstractions;
using Domain.Abstractions;
using Domain.Entities.ChatAgregado;
using Domain.Entities.ConsultaAgregado;
using Domain.Entities.DocumentoAgregado;
using Domain.Services;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
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
                Titulo = "Consulta1",
                Descripcion = "Descripcion1",
                EmbeddingTitulo = [1, 2, 3],
                EmbeddingDescripcion = [4, 5, 6],
                Solucion = "Solucion1",
            };

            var consultaNoSimilar = new Consulta
            {
                RemoteId = 2,
                Titulo = "Consulta2",
                Descripcion = "Descripcion2",
                EmbeddingTitulo = [7, 8, 9],
                EmbeddingDescripcion = [10, 11, 12],
                Solucion = "Solucion2",
            };

            var consultas = new List<Consulta>
            {
                consultaSimilar,
                consultaNoSimilar
            };

            var documentoRelacionado = new Document
            {
                Filename = "fileName1",
                Texto = "Documento1",
            };

            var documentoNoRelacionado = new Document
            {
                Filename = "fileName2",
                Texto = "Documento2",
            };

            var documentos = new List<Document>
            {
                documentoRelacionado,
                documentoRelacionado,
                documentoNoRelacionado
            };

            var llamadaHerramienta = new MensajeLlamadaHerramienta
            {
                FunctionName = "informacion",
                PluginName = "InformacionTool",
                Argumentos =
                    new Dictionary<string, object?> { ["pregunta"] = consulta },
                DateTime = DateTime.Now
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
                .ReturnsAsync(documentos.Take(2).ToList());

            var consultasPlugin = new InformacionTool(
                Mock.Of<ILogger<InformacionTool>>(),
                embeddingServiceMock.Object,
                consultaRepository.Object,
                documentRepository.Object,
                rankerMock.Object);

            // Act
            var result = await consultasPlugin
                .BuscarInformacionAsync(consulta, llamadaHerramienta)
                .ConfigureAwait(false);

            // Assert
            Assert.That(result, Is.TypeOf<MensajeHerramientaInfo>());
            Assert.That(result.DocumentosRecuperados, Has.Count.EqualTo(3));
            Assert.That(result.ConsultasRecuperadas, Has.Count.EqualTo(2));
            Assert.That(result.Llamada, Is.EqualTo(llamadaHerramienta));

            // Verify ranked documents
            Assert.That(
                result.DocumentosRecuperados
                    .Count(
                        dr => dr.DocumentoId == documentoRelacionado.Id &&
                                dr.Rank == true),
                Is.EqualTo(2));
            Assert.That(
                result.DocumentosRecuperados
                    .Any(
                        dr => dr.DocumentoId == documentoNoRelacionado.Id &&
                                dr.Rank == false));

            // Verify ranked consultas
            Assert.That(
                result.ConsultasRecuperadas
                    .Any(
                        cr => cr.ConsultaId == consultaSimilar.Id &&
                                cr.Rank == true));
            Assert.That(
                result.ConsultasRecuperadas
                    .Any(
                        cr => cr.ConsultaId == consultaNoSimilar.Id &&
                                cr.Rank == false));
        }

        [Test]
        public async Task BuscarInformacionAsync_NoDocumentsNoConsultasTest()
        {
            // Arrange
            var embeddingServiceMock = new Mock<IEmbeddingService>();
            var consultaRepository = new Mock<IConsultaRepository>();
            var documentRepository = new Mock<IDocumentRepository>();
            var rankerMock = new Mock<IRanker>();

            var consulta = "consulta";
            var embeddingConsulta = new float[] { 1, 2, 3 };

            var llamadaHerramienta = new MensajeLlamadaHerramienta
            {
                FunctionName = "informacion",
                PluginName = "InformacionTool",
                Argumentos =
                    new Dictionary<string, object?> { ["pregunta"] = consulta },
                DateTime = DateTime.Now
            };

            embeddingServiceMock
                .Setup(x => x.GenerateAsync(consulta))
                .ReturnsAsync(embeddingConsulta);

            consultaRepository
                .Setup(x => x.GetConsultasSimilaresAsync(embeddingConsulta))
                .ReturnsAsync([]);

            documentRepository.Setup(
                x => x.GetDocumentosRelacionadosAsync(embeddingConsulta))
                .ReturnsAsync([]);

            rankerMock.Setup(
                x => x.RankAsync(It.IsAny<List<Consulta>>(), consulta))
                .ReturnsAsync([]);

            rankerMock.Setup(
                x => x.RankAsync(It.IsAny<List<Document>>(), consulta))
                .ReturnsAsync([]);

            var consultasPlugin = new InformacionTool(
                Mock.Of<ILogger<InformacionTool>>(),
                embeddingServiceMock.Object,
                consultaRepository.Object,
                documentRepository.Object,
                rankerMock.Object);

            // Act
            var result = await consultasPlugin
                .BuscarInformacionAsync(consulta, llamadaHerramienta)
                .ConfigureAwait(false);

            // Assert
            Assert.That(result, Is.TypeOf<MensajeHerramientaInfo>());
            Assert.That(result.DocumentosRecuperados, Is.Empty);
            Assert.That(result.ConsultasRecuperadas, Is.Empty);
            Assert.That(result.Llamada, Is.EqualTo(llamadaHerramienta));
        }
    }
}
