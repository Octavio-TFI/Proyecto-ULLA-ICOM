namespace Infrastructure.Outbox.Tests
{
    internal class OutboxPublisherTests
    {
        JsonSerializerSettings _jsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        [Test]
        public async Task PublishOutboxEventAsync_Succeded()
        {
            // Arrange
            var mensajeRecibidoEvent = new MensajeRecibidoEvent
            {
                EntityId = Guid.NewGuid()
            };

            var outboxEvent = new OutboxEvent
            {
                EventType = "EventType",
                EventData =
                    JsonConvert.SerializeObject(
                        mensajeRecibidoEvent,
                        _jsonSettings),
                OccurredOn = DateTime.Now,
                MaxRetries = 5,
                RetryIntervalSeconds = 10,
                NextRetryOn = DateTime.Now
            };

            var context = new Mock<ChatContext>();
            var publisher = new Mock<IPublisher>();
            var logger = new Mock<ILogger<OutboxPublisher>>();
            var outboxPublisher = new OutboxPublisher(
                publisher.Object,
                logger.Object,
                context.Object);

            // Act
            await outboxPublisher.PublishOutboxEventsAsync(
                outboxEvent,
                CancellationToken.None);

            // Assert
            logger.VerifyLog().ErrorWasNotCalled();

            publisher.Verify(
                p => p.Publish(
                    It.IsAny<INotification>(),
                    CancellationToken.None),
                Times.Once);

            context.Verify(c => c.Update(outboxEvent), Times.Once);
            context.Verify(
                c => c.SaveChangesAsync(CancellationToken.None),
                Times.Once);

            Assert.Multiple(
                () =>
                {
                    Assert.That(outboxEvent.IsProcessed, Is.True);
                    Assert.That(
                        outboxEvent.ProcessedOn,
                        Is.Not.Null.And.GreaterThan(outboxEvent.OccurredOn));
                    Assert.That(outboxEvent.RetryCount, Is.EqualTo(0));
                });
        }

        [Test]
        public async Task PublishOutboxEventAsync_ErrorPublishingEvent_IncrementsRetryCount(
            )
        {
            // Arrange
            var now = DateTime.Now;

            var mensajeRecibidoEvent = new MensajeRecibidoEvent
            {
                EntityId = Guid.NewGuid()
            };
            var outboxEvent = new OutboxEvent
            {
                EventType = "EventType",
                EventData =
                    JsonConvert.SerializeObject(
                        mensajeRecibidoEvent,
                        _jsonSettings),
                OccurredOn = now,
                RetryCount = 2,
                MaxRetries = 5,
                RetryIntervalSeconds = 10,
                NextRetryOn = now
            };

            var context = new Mock<ChatContext>();
            var publisher = new Mock<IPublisher>();
            var logger = new Mock<ILogger<OutboxPublisher>>();
            var outboxPublisher = new OutboxPublisher(
                publisher.Object,
                logger.Object,
                context.Object);

            publisher.Setup(
                p => p.Publish(
                    It.IsAny<INotification>(),
                    CancellationToken.None))
                .Throws<Exception>();

            // Act
            await outboxPublisher.PublishOutboxEventsAsync(
                outboxEvent,
                CancellationToken.None);

            // Assert
            logger.VerifyLog().ErrorWasCalled();

            context.Verify(c => c.Update(outboxEvent), Times.Once);
            context.Verify(
                c => c.SaveChangesAsync(CancellationToken.None),
                Times.Once);

            Assert.Multiple(
                () =>
                {
                    Assert.That(outboxEvent.IsProcessed, Is.False);
                    Assert.That(outboxEvent.ProcessedOn, Is.Null);
                    Assert.That(outboxEvent.RetryCount, Is.EqualTo(3));
                    Assert.That(
                        outboxEvent.NextRetryOn,
                        Is.Not.Null.And
                                .GreaterThan(
                                    now.AddSeconds(outboxEvent.RetryIntervalSeconds)));
                });
        }

        [Test]
        public async Task PublishOutboxEventAsync_ErrorPublishingEvent_MaxRetriesExceeded(
            )
        {
            // Arrange
            var mensajeRecibidoEvent = new MensajeRecibidoEvent
            {
                EntityId = Guid.NewGuid()
            };
            var outboxEvent = new OutboxEvent
            {
                EventType = "EventType",
                EventData =
                    JsonConvert.SerializeObject(
                        mensajeRecibidoEvent,
                        _jsonSettings),
                OccurredOn = DateTime.Now,
                RetryCount = 4,
                MaxRetries = 5,
                RetryIntervalSeconds = 10,
                NextRetryOn = DateTime.Now
            };

            var context = new Mock<ChatContext>();
            var publisher = new Mock<IPublisher>();
            var logger = new Mock<ILogger<OutboxPublisher>>();
            var outboxPublisher = new OutboxPublisher(
                publisher.Object,
                logger.Object,
                context.Object);

            publisher.Setup(
                p => p.Publish(
                    It.IsAny<INotification>(),
                    CancellationToken.None))
                .Throws<Exception>();

            // Act
            await outboxPublisher.PublishOutboxEventsAsync(
                outboxEvent,
                CancellationToken.None);

            // Assert
            logger.VerifyLog().ErrorWasCalled();

            context.Verify(c => c.Update(outboxEvent), Times.Once);
            context.Verify(
                c => c.SaveChangesAsync(CancellationToken.None),
                Times.Once);

            Assert.Multiple(
                () =>
                {
                    Assert.That(outboxEvent.IsProcessed, Is.True);
                    Assert.That(outboxEvent.ProcessedOn, Is.Not.Null);
                    Assert.That(outboxEvent.RetryCount, Is.EqualTo(5));
                });
        }

        [Test]
        public async Task PublishOutboxEventAsync_ErrorDeserializingEvent()
        {
            // Arrange
            var outboxEvent = new OutboxEvent
            {
                EventType = "EventType",
                EventData = "Invalid JSON",
                OccurredOn = DateTime.Now,
                MaxRetries = 5,
                RetryIntervalSeconds = 10,
                NextRetryOn = DateTime.Now
            };

            var context = new Mock<ChatContext>();
            var publisher = new Mock<IPublisher>();
            var logger = new Mock<ILogger<OutboxPublisher>>();
            var outboxPublisher = new OutboxPublisher(
                publisher.Object,
                logger.Object,
                context.Object);

            // Act
            await outboxPublisher.PublishOutboxEventsAsync(
                outboxEvent,
                CancellationToken.None);

            // Assert
            logger.VerifyLog().ErrorWasCalled();

            context.Verify(c => c.Update(outboxEvent), Times.Once);
            context.Verify(
                c => c.SaveChangesAsync(CancellationToken.None),
                Times.Once);

            Assert.Multiple(
                () =>
                {
                    Assert.That(outboxEvent.RetryCount, Is.EqualTo(1));
                    Assert.That(outboxEvent.NextRetryOn, Is.Not.Null);
                });
        }

        [Test]
        public async Task PublishOutboxEventAsync_EventIsNull()
        {
            // Arrange
            var outboxEvent = new OutboxEvent
            {
                EventType = "EventType",
                EventData =
                    JsonConvert.SerializeObject(
                        null!,
                        _jsonSettings),
                OccurredOn = DateTime.Now,
                MaxRetries = 5,
                RetryIntervalSeconds = 10,
                NextRetryOn = DateTime.Now
            };

            var context = new Mock<ChatContext>();
            var publisher = new Mock<IPublisher>();
            var logger = new Mock<ILogger<OutboxPublisher>>();
            var outboxPublisher = new OutboxPublisher(
                publisher.Object,
                logger.Object,
                context.Object);

            // Act
            await outboxPublisher.PublishOutboxEventsAsync(
                outboxEvent,
                CancellationToken.None);

            // Assert
            logger.VerifyLog().ErrorWasCalled();

            context.Verify(c => c.Update(outboxEvent), Times.Once);
            context.Verify(
                c => c.SaveChangesAsync(CancellationToken.None),
                Times.Once);

            Assert.Multiple(
                () =>
                {
                    Assert.That(outboxEvent.RetryCount, Is.EqualTo(1));
                    Assert.That(outboxEvent.NextRetryOn, Is.Not.Null);
                });
        }
    }
}
