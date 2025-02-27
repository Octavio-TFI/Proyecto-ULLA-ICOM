using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.DocumentProcessing.Tests
{
    internal class DocumentProcessorServiceTests
    {
        [Test]
        public async Task ExecuteAsyncTest()
        {
            // Arrange
            string[] files = ["file1", "file2"];
            byte[][] bytes = [new byte[1], new byte[2]];
            Document document1 = new()
            {
                Filename = "file1",
                Texto = "TituloFile1"
            };
            Document document2 = new()
            {
                Filename = "file2",
                Texto = "TextoFile2"
            };

            var services = new ServiceCollection();
            var directoryManager = new Mock<IDirectoryManager>();
            var pdfProcessor = new Mock<IDocumentProcessor>();
            var txtProcessor = new Mock<IDocumentProcessor>();
            var pathManager = new Mock<IPathManager>();
            var fileManager = new Mock<IFileManager>();
            var logger = new Mock<ILogger<DocumentProcessorService>>();
            var documentRepository = new Mock<IDocumentRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            directoryManager.Setup(x => x.GetFiles("Documentacion"))
                .Returns(files);

            documentRepository.Setup(x => x.GetAllFilenamesAsync())
                .ReturnsAsync(["file2"]);

            pathManager.Setup(x => x.GetExtension("file1")).Returns("pdf");
            pathManager.Setup(x => x.GetExtension("file2")).Returns("txt");

            fileManager.Setup(x => x.ReadAllBytesAsync("file1"))
                .ReturnsAsync(bytes[0]);
            fileManager.Setup(x => x.ReadAllBytesAsync("file2"))
                .ReturnsAsync(bytes[1]);

            pdfProcessor.Setup(x => x.ProcessAsync("file1", bytes[0]))
                .ReturnsAsync(document1);
            txtProcessor.Setup(x => x.ProcessAsync("file2", bytes[1]))
                .ReturnsAsync(document2);

            services.AddKeyedSingleton("pdf", pdfProcessor.Object);
            services.AddKeyedSingleton("txt", txtProcessor.Object);
            services.AddSingleton(documentRepository.Object);
            services.AddKeyedSingleton(Contexts.Embedding, unitOfWork.Object);

            var documentProcessorService = new DocumentProcessorService(
                services.BuildServiceProvider(),
                directoryManager.Object,
                pathManager.Object,
                fileManager.Object,
                logger.Object);

            // Act
            await documentProcessorService.StartAsync(CancellationToken.None);

            // Assert
            documentRepository.Verify(x => x.InsertAsync(document1), Times.Once);
            documentRepository.Verify(
                x => x.InsertAsync(document2),
                Times.Never);

            unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);

            await documentProcessorService.StopAsync(CancellationToken.None);
        }

        [Test]
        public async Task ExecuteAsync_ExceptionTest()
        {
            string[] files = ["file1"];
            byte[][] bytes = [new byte[1]];
            Document document1 = new()
            {
                Filename = "file1",
                Texto = "TituloFile1"
            };
            Document document2 = new()
            {
                Filename = "file2",
                Texto = "TextoFile2"
            };

            var services = new ServiceCollection();
            var directoryManager = new Mock<IDirectoryManager>();
            var pdfProcessor = new Mock<IDocumentProcessor>();
            var pathManager = new Mock<IPathManager>();
            var fileManager = new Mock<IFileManager>();
            var logger = new Mock<ILogger<DocumentProcessorService>>();
            var documentRepository = new Mock<IDocumentRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            directoryManager.Setup(x => x.GetFiles("Documentacion"))
                .Returns(files);

            documentRepository.Setup(x => x.GetAllFilenamesAsync())
                .ReturnsAsync(["file2"]);

            pathManager.Setup(x => x.GetExtension("file1")).Returns("pdf");

            fileManager.Setup(x => x.ReadAllBytesAsync("file1"))
                .ReturnsAsync(bytes[0]);

            pdfProcessor.Setup(x => x.ProcessAsync("file1", bytes[0]))
                .Throws<Exception>();

            services.AddKeyedSingleton("pdf", pdfProcessor.Object);
            services.AddSingleton(documentRepository.Object);
            services.AddKeyedSingleton(Contexts.Embedding, unitOfWork.Object);

            var documentProcessorService = new DocumentProcessorService(
                services.BuildServiceProvider(),
                directoryManager.Object,
                pathManager.Object,
                fileManager.Object,
                logger.Object);

            // Act
            await documentProcessorService.StartAsync(CancellationToken.None);

            // Assert
            documentRepository.Verify(
                x => x.InsertAsync(document1),
                Times.Never);

            unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);

            await documentProcessorService.StopAsync(CancellationToken.None);
        }
    }
}
