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
                Texto = "TituloFile1",
                Embedding = [1,2,3]
            }, new Document { Texto = "TextoFile1", Embedding = [3,4,5] }];

            var services = new ServiceCollection();
            var directoryManager = new Mock<IDirectoryManager>();
            var pdfProcessor = new Mock<IDocumentProcessor>();
            var pathManager = new Mock<IPathManager>();
            var documentRepository = new Mock<IDocumentRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            directoryManager.Setup(x => x.GetFiles("./Documentacion"))
                .Returns(files);

            pathManager.Setup(x => x.GetExtension("file1")).Returns("pdf");

            pdfProcessor.Setup(x => x.ProcessAsync("file1"))
                .ReturnsAsync(documentsFile1);

            services.AddKeyedSingleton("pdf", pdfProcessor.Object);

            var documentProcessorService = new DocumentProcessorService(
                services.BuildServiceProvider(),
                directoryManager.Object,
                pathManager.Object,
                documentRepository.Object,
                unitOfWork.Object);

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
