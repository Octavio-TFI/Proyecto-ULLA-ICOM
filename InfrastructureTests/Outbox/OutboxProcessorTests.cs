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

            outboxPublisherChat.Verify(
                x => x.PublishOutboxEventsAsync(
                    It.Is<OutboxEvent>(e => e.Id == outboxEvents.Last().Id),
                    CancellationToken.None),
                Times.Never);
        }
    }
}
