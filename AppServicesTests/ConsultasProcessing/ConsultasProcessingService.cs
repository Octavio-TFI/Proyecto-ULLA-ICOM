﻿using AppServices.DocumentProcessing;
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
    internal class ConsultasProcessingService
    {
        [Test]
        public async Task ExecuteAsyncTest()
        {
            // Arrange
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
            },
            new()
            {
                Id = 3,
                Titulo = "Titulo3",
                Descripcion = "Descripcion3",
                PreFixes = [],
                Fix = "Fix3"
            }];

            var consulta = new Consulta
            {
                Id = 1,
                Titulo = "Titulo1",
                Descripcion = "Descripcion1",
                Solucion = "Solucion1",
                EmbeddingTitulo = new float[1],
                EmbeddingDescripcion = new float[1],
            };

            var services = new ServiceCollection();
            var consultaDataRepositoryMock = new Mock<IConsultaDataRepository>();
            var consultaProcessorMock = new Mock<IConsultaProcessor>();
            var loggerMock = new Mock<ILogger<ConsultasProcesorService>>();
            var consultaRepositoryMock = new Mock<IConsultaRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            consultaDataRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(consultasDatas);

            consultaRepositoryMock.Setup(x => x.GetAllIdsAsync())
                .ReturnsAsync([1]);

            consultaProcessorMock.Setup(x => x.ProcessAsync(consultasDatas[1]))
                .ReturnsAsync(consulta);

            consultaProcessorMock.Setup(x => x.ProcessAsync(consultasDatas[2]))
                .ThrowsAsync(new Exception());

            services.AddSingleton(consultaRepositoryMock.Object);
            services.AddKeyedSingleton(
                Contexts.Embedding,
                unitOfWorkMock.Object);

            var consultasProcesorService = new ConsultasProcesorService(
                services.BuildServiceProvider(),
                consultaDataRepositoryMock.Object,
                consultaProcessorMock.Object,
                loggerMock.Object);

            // Act
            await consultasProcesorService.StartAsync(CancellationToken.None);

            // Assert
            consultaRepositoryMock.Verify(
                x => x.InsertAsync(consulta),
                Times.Once);
            consultaRepositoryMock.Verify(
                x => x.InsertAsync(It.IsAny<Consulta>()),
                Times.Once);

            unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);

            await consultasProcesorService.StopAsync(CancellationToken.None);
        }
    }
}
