using Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Tests
{
    internal class DocumentProcessorServiceTests
    {
        [Test]
        public async Task ExecuteAsyncTest()
        {
            // Arrange
            string[] files = ["file1", "file2"];
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
            var documentRepository = new Mock<IDocumentRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            directoryManager.Setup(x => x.GetFiles("./Documentacion"))
                .Returns(files);

            pathManager.Setup(x => x.GetExtension("file1")).Returns("pdf");
            pathManager.Setup(x => x.GetExtension("file2")).Returns("txt");

            pdfProcessor.Setup(x => x.ProcessAsync("file1"))
                .ReturnsAsync(documentsFile1);
            txtProcessor.Setup(x => x.ProcessAsync("file2"))
                .ReturnsAsync(documentsFile2);

            services.AddKeyedSingleton("pdf", pdfProcessor.Object);
            services.AddKeyedSingleton("txt", txtProcessor.Object);
            services.AddSingleton(documentRepository.Object);
            services.AddKeyedSingleton(Contexts.Embedding, unitOfWork.Object);

            var documentProcessorService = new DocumentProcessorService(
                services.BuildServiceProvider(),
                directoryManager.Object,
                pathManager.Object);

            // Act
            await documentProcessorService.StartAsync(CancellationToken.None);

            // Assert
            documentRepository.Verify(
                x => x.InsertRangeAsync(documentsFile1),
                Times.Once);
            unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));

            await documentProcessorService.StopAsync(CancellationToken.None);
        }
    }
}
