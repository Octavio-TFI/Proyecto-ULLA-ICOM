using Infrastructure.Outbox;
using Infrastructure.Outbox.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Outbox.Tests
{
    internal class OutboxServiceTests
    {
        [Test]
        public async Task ExecuteAsync()
        {
            // Arrange
            var outboxProcessorMock = new Mock<IOutboxProcessor>();
            var outboxService = new OutboxService(outboxProcessorMock.Object);

            var cts = new CancellationTokenSource();

            // Act
            var task = outboxService.StartAsync(cts.Token);

            await Task.Delay(2100);

            cts.Cancel();
            await task;

            // Assert
            outboxProcessorMock.Verify(
                x => x.ProcessOutboxAsync(It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        }
    }
}
