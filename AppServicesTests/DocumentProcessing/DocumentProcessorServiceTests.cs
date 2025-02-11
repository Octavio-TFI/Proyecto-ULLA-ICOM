using AppServices.DocumentProcessing;
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
            string[] files = ["file1", "file2", "file3"];
            byte[][] bytes = [new byte[1], new byte[2], new byte[3]];
            List<Document> documentsFile1 = [ new Document
            {
                Filename = "file1",
                Texto = "TituloFile1",
                Embedding = [1,2,3]
            }, new Document
            {
                Filename = "file1",
                Texto = "TextoFile1",
                Embedding = [3,4,5]
            }];
            List<Document> documentsFile2 = [new Document()
            {
                Filename = "file2",
                Texto = "TextoFile2",
                Embedding = [6,7,8]
            }];

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
                .ReturnsAsync(["file3"]);

            pathManager.Setup(x => x.GetExtension("file1")).Returns("pdf");
            pathManager.Setup(x => x.GetExtension("file2")).Returns("txt");

            fileManager.Setup(x => x.ReadAllBytesAsync("file1"))
                .ReturnsAsync(bytes[0]);
            fileManager.Setup(x => x.ReadAllBytesAsync("file2"))
                .ReturnsAsync(bytes[1]);

            pdfProcessor.Setup(x => x.ProcessAsync("file1", bytes[0]))
                .ReturnsAsync(documentsFile1);
            txtProcessor.Setup(x => x.ProcessAsync("file2", bytes[1]))
                .ReturnsAsync(documentsFile2);

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
            pathManager.Verify(x => x.GetExtension("file3"), Times.Never);

            documentRepository.Verify(
                x => x.InsertRangeAsync(documentsFile1),
                Times.Once);
            documentRepository.Verify(
                x => x.InsertRangeAsync(documentsFile2),
                Times.Once);

            unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));

            await documentProcessorService.StopAsync(CancellationToken.None);
        }
    }
}
