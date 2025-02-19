using AppServices.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.MesaDeAyuda.Tests
{
    internal class ConsultaDataRepositoryTests
    {
        [Test]
        public async Task GetAllAsync()
        {
            // Arrange
            var fileManagerMock = new Mock<IFileManager>();

            fileManagerMock.Setup(x => x.ReadAllTextAsync("MesaDeAyuda.xml"))
                .ReturnsAsync(string.Empty);


            var consultaDataRepository = new ConsultaDataRepository(
                fileManagerMock.Object);

            // Act
            var consultaDatas = await consultaDataRepository.GetAllAsync();

            // Assert
            Assert.That(consultaDatas, Is.Empty);
        }
    }
}
