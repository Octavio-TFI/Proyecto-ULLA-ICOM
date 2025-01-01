using Infrastructure.Database.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Tests
{
    internal class UnitOfWorkTests
    {
        [Test]
        public async Task SaveChangesAsync_ShouldSaveChanges()
        {
            // Arrange
            var contextMock = new Mock<BaseContext>();
            var unitOfWork = new UnitOfWork<BaseContext>(contextMock.Object);

            // Act
            await unitOfWork.SaveChangesAsync();

            // Assert
            contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }
    }
}
