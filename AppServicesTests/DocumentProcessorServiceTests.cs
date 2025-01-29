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

            var directoryManager = new Mock<IDirectoryManager>();
            directoryManager.Setup(x => x.GetFiles("./Documentacion"))
                .Returns(files);

            var documentProcessorService = new DocumentProcessorService(
                directoryManager.Object);

            // Act
            await documentProcessorService.StartAsync(CancellationToken.None);

            // Assert
            await documentProcessorService.StopAsync(CancellationToken.None);
        }
    }
}
