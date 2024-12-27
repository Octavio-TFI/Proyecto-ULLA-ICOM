using InfrastructureTests.Database.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Outbox.Tests
{
    internal class OutboxProcessorTests
    {
        [Test]
        public async Task ProcessOutboxAsyncTest()
        {
            // Arrange
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            var outboxPublisher = new Mock<IOutboxPublisher>();
            var chatContext = DatabaseTestsHelper.CreateInMemoryContext();

            var serviceScopeMock = new Mock<IServiceScope>();
            var serviceProvider = new ServiceCollection()
                .AddSingleton(chatContext)
                .BuildServiceProvider();

            serviceScopeMock.Setup(x => x.ServiceProvider)
                .Returns(serviceProvider);

            serviceScopeFactory.Setup(x => x.CreateScope())
                .Returns(serviceScopeMock.Object);

            var outboxEvents = new List<OutboxEvent>
            {
                new()
                {
                    EventType = "a",
                    EventData = "data1",
                    OccurredOn = DateTime.Now
                },
                new()
                {
                    EventType = "b",
                    EventData = "data2",
                    OccurredOn = DateTime.Now
                },
                new()
                {
                    EventType = "c",
                    EventData = "data3",
                    OccurredOn = DateTime.Now,
                    IsProcessed = true
                },
            };

            chatContext.OutboxEvents.AddRange(outboxEvents);
            await chatContext.SaveChangesAsync();

            var outboxProcessor = new OutboxProcessor(
                serviceScopeFactory.Object,
                outboxPublisher.Object);

            // Act
            await outboxProcessor.ProcessOutboxAsync(CancellationToken.None);

            // Assert
            foreach(var outboxEvent in outboxEvents.Take(2))
            {
                outboxPublisher.Verify(
                    x => x.PublishOutboxEventsAsync(
                        outboxEvent,
                        chatContext,
                        CancellationToken.None),
                    Times.Once);
            }

            outboxPublisher.Verify(
                x => x.PublishOutboxEventsAsync(
                    outboxEvents.Last(),
                    chatContext,
                    CancellationToken.None),
                Times.Never);
        }
    }
}
