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
            var outboxPublisherChat = new Mock<IOutboxPublisher>();

            var chatContext = DatabaseTestsHelper.CreateInMemoryChatContext();

            var serviceScopeMock = new Mock<IServiceScope>();
            var serviceProvider = new ServiceCollection()
                .AddSingleton(chatContext)
                .AddSingleton(outboxPublisherChat.Object)
                .BuildServiceProvider();

            serviceScopeMock.Setup(x => x.ServiceProvider)
                .Returns(serviceProvider);

            serviceScopeFactory.Setup(x => x.CreateScope())
                .Returns(serviceScopeMock.Object);

            var now = DateTime.Now;
            var outboxEvents = new List<OutboxEvent>
            {
                new()
                {
                    EventType = "a",
                    EventData = "data1",
                    OccurredOn = now.AddMinutes(-5),
                    MaxRetries = 3,
                    RetryIntervalSeconds = 10,
                    NextRetryOn = now.AddSeconds(-1)
                },
                new()
                {
                    EventType = "b",
                    EventData = "data2",
                    OccurredOn = now.AddMinutes(-4),
                    MaxRetries = 3,
                    RetryIntervalSeconds = 10,
                    NextRetryOn = now.AddSeconds(-2)
                },
                new()
                {
                    EventType = "c",
                    EventData = "data3",
                    OccurredOn = now.AddMinutes(-3),
                    IsProcessed = true,
                    MaxRetries = 3,
                    RetryIntervalSeconds = 10,
                    NextRetryOn = now.AddSeconds(-3)
                },
                new()
                {
                    EventType = "d",
                    EventData = "data4",
                    OccurredOn = now.AddMinutes(-2),
                    MaxRetries = 3,
                    RetryIntervalSeconds = 10,
                    NextRetryOn = now.AddMinutes(5) // future, should not process
                },
            };

            chatContext.OutboxEvents.AddRange(outboxEvents);
            await chatContext.SaveChangesAsync();

            var outboxProcessor = new OutboxProcessor(
                serviceScopeFactory.Object);

            // Act
            await outboxProcessor.ProcessOutboxAsync(CancellationToken.None);

            // Assert
            foreach (var outboxEvent in outboxEvents.Take(2))
            {
                outboxPublisherChat.Verify(
                    x => x.PublishOutboxEventsAsync(
                        It.Is<OutboxEvent>(e => e.Id == outboxEvent.Id),
                        CancellationToken.None),
                    Times.Once);
            }

            // Verify that the processed event is not published again
            outboxPublisherChat.Verify(
                x => x.PublishOutboxEventsAsync(
                    It.Is<OutboxEvent>(e => e.EventType == "c"),
                    CancellationToken.None),
                Times.Never);

            // Verify that the future event is not published
            outboxPublisherChat.Verify(
                x => x.PublishOutboxEventsAsync(
                    It.Is<OutboxEvent>(e => e.EventType == "d"),
                    CancellationToken.None),
                Times.Never);
        }
    }
}
