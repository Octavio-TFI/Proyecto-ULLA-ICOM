using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.ConsultasProcessing.Tests
{
    internal class ConsultasProcessingServiceTests
    {
        [Test]
        public async Task ExecuteAsyncTest()
        {
            // Arrange
            int[] existingIds = [3];

            List<ConsultaData> consultasDatas = [
            new()
            {
                Id = 1,
                Titulo = "Titulo1",
                Descripcion = "Descripcion1",
                PreFixes = [],
                Fix = "Fix1",
            },
            new()
            {
                Id = 2,
                Titulo = "Titulo2",
                Descripcion = "Descripcion2",
                PreFixes = [],
                Fix = "Fix2"
            }];

            var consulta = new Consulta
            {
                RemoteId = 1,
                Titulo = "Titulo1",
                Descripcion = "Descripcion1",
                Solucion = "Solucion1",
                EmbeddingTitulo = new float[1],
                EmbeddingDescripcion = new float[1],
            };

            var services = new ServiceCollection();
            var consultaDataRepositoryMock = new Mock<IConsultaDataRepository>();
            var consultaFactoryMock = new Mock<IConsultaFactory>();
            var loggerMock = new Mock<ILogger<ConsultasProcesorService>>();
            var consultaRepositoryMock = new Mock<IConsultaRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            consultaRepositoryMock.Setup(x => x.GetAllIdsAsync())
                .ReturnsAsync(existingIds);

            consultaDataRepositoryMock.Setup(
                x => x.GetAllExceptExistingIdsAsync(existingIds))
                .ReturnsAsync(consultasDatas);

            consultaFactoryMock.Setup(x => x.CreateAsync(consultasDatas[0]))
                .ReturnsAsync(consulta);

            consultaFactoryMock.Setup(x => x.CreateAsync(consultasDatas[1]))
                .ThrowsAsync(new Exception());

            services.AddSingleton(consultaRepositoryMock.Object);
            services.AddKeyedSingleton(
                Contexts.Embedding,
                unitOfWorkMock.Object);

            var consultasProcesorService = new ConsultasProcesorService(
                services.BuildServiceProvider(),
                consultaDataRepositoryMock.Object,
                consultaFactoryMock.Object,
                loggerMock.Object);

            // Act
            await consultasProcesorService.StartAsync(CancellationToken.None)
                .ConfigureAwait(false);

            // Assert
            consultaRepositoryMock.Verify(
                x => x.InsertAsync(consulta),
                Times.Once);
            consultaRepositoryMock.Verify(
                x => x.InsertAsync(It.IsAny<Consulta>()),
                Times.Once);

            unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);

            await consultasProcesorService.StopAsync(CancellationToken.None)
                .ConfigureAwait(false);
        }
    }
}
