using Domain.Abstractions;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Factories.Tests
{
    class ConsultaFactoryTests
    {
        [Test]
        public async Task ProcessAsync_Success()
        {
            // Arrange
            var consultaData = new ConsultaData
            {
                Id = 1,
                Titulo = "Test Title",
                Descripcion = "Test Description",
                PreFixes = ["Prefix1", "Prefix2"],
                Fix = "Test Fix"
            };

            var agentMock = new Mock<IAgent>();
            var embeddingServiceMock = new Mock<IEmbeddingService>();

            agentMock.Setup(
                x => x.GenerarRespuestaAsync(consultaData.ToString(), null))
                .ReturnsAsync(
                    JsonConvert.SerializeObject(
                        new ConsultaResumen
                        {
                            Descripcion = "Processed Description",
                            Solucion = "Processed Solution"
                        }));

            embeddingServiceMock.Setup(
                e => e.GenerateAsync(
                    It.Is<IList<string>>(
                        x => x.First() == consultaData.Titulo &&
                            x.Last() == "Processed Description")))
                .ReturnsAsync(
                    [[0.1f, 0.2f],[0.3f, 0.4f]]);

            var consultaFactory = new ConsultaFactory(
                agentMock.Object,
                embeddingServiceMock.Object);

            // Act
            var result = await consultaFactory.CreateAsync(consultaData)
                .ConfigureAwait(false);

            // Assert
            Assert.That(result.Id, Is.Not.Null.And.Not.EqualTo(Guid.Empty));
            Assert.That(result.RemoteId, Is.EqualTo(consultaData.Id));
            Assert.That(result.Titulo, Is.EqualTo(consultaData.Titulo));
            Assert.That(result.Descripcion, Is.EqualTo("Processed Description"));
            Assert.That(result.Solucion, Is.EqualTo("Processed Solution"));
            Assert.That(
                result.EmbeddingTitulo,
                Is.EqualTo(new float[] { 0.1f, 0.2f }));
            Assert.That(
                result.EmbeddingDescripcion,
                Is.EqualTo(new float[] { 0.3f, 0.4f }));
        }

        [Test]
        public void ProcessAsync_DeserializationError_ThrowsException()
        {
            // Arrange
            var consultaData = new ConsultaData
            {
                Id = 1,
                Titulo = "Test Title",
                Descripcion = "Test Description",
                PreFixes = ["Prefix1", "Prefix2"],
                Fix = "Test Fix"
            };

            var agentMock = new Mock<IAgent>();
            var embeddingServiceMock = new Mock<IEmbeddingService>();

            agentMock.Setup(
                x => x.GenerarRespuestaAsync(consultaData.ToString(), null))
                .ReturnsAsync(string.Empty);

            var consultaFactory = new ConsultaFactory(
                agentMock.Object,
                embeddingServiceMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(
                () => consultaFactory.CreateAsync(consultaData));
        }
    }
}
