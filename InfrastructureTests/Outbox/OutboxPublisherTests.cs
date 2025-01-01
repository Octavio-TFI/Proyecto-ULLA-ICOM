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
            var mensajeRecibidoEvent = new MensajeRecibidoEvent { ChatId = 1 };

            var outboxEvent = new OutboxEvent
            {
                EventType = "EventType",
                EventData =
                    JsonConvert.SerializeObject(
                        mensajeRecibidoEvent,
                        _jsonSettings),
                OccurredOn = DateTime.Now
            };

            var context = new Mock<DbContext>();
            var publisher = new Mock<IPublisher>();
            var logger = new Mock<ILogger<OutboxPublisher>>();
            var outboxPublisher = new OutboxPublisher(
                publisher.Object,
                logger.Object);

            // Act
            await outboxPublisher.PublishOutboxEventsAsync(
                outboxEvent,
                context.Object,
                CancellationToken.None);

            // Assert
            logger.VerifyLog().ErrorWasNotCalled();

            publisher.Verify(
                p => p.Publish(
                    It.IsAny<INotification>(),
                    CancellationToken.None),
                Times.Once);
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
                });
        }

        [Test]
        public async Task PublishOutboxEventAsync_ErrorPublishingEvent()
        {
            // Arrange
            var mensajeRecibidoEvent = new MensajeRecibidoEvent { ChatId = 1 };
            var outboxEvent = new OutboxEvent
            {
                EventType = "EventType",
                EventData =
                    JsonConvert.SerializeObject(
                        mensajeRecibidoEvent,
                        _jsonSettings),
                OccurredOn = DateTime.Now
            };

            var context = new Mock<DbContext>();
            var publisher = new Mock<IPublisher>();
            var logger = new Mock<ILogger<OutboxPublisher>>();
            var outboxPublisher = new OutboxPublisher(
                publisher.Object,
                logger.Object);

            publisher.Setup(
                p => p.Publish(
                    It.IsAny<INotification>(),
                    CancellationToken.None))
                .Throws<Exception>();

            // Act
            await outboxPublisher.PublishOutboxEventsAsync(
                outboxEvent,
                context.Object,
                CancellationToken.None);

            // Assert
            logger.VerifyLog()
                .ErrorWasCalled()
                .MessageEquals("Error al publicar el evento de Outbox");

            context.Verify(
                c => c.SaveChangesAsync(CancellationToken.None),
                Times.Never);

            Assert.Multiple(
                () =>
                {
                    Assert.That(outboxEvent.IsProcessed, Is.False);
                    Assert.That(outboxEvent.ProcessedOn, Is.Null);
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
                OccurredOn = DateTime.Now
            };

            var context = new Mock<DbContext>();
            var publisher = new Mock<IPublisher>();
            var logger = new Mock<ILogger<OutboxPublisher>>();
            var outboxPublisher = new OutboxPublisher(
                publisher.Object,
                logger.Object);

            // Act
            await outboxPublisher.PublishOutboxEventsAsync(
                outboxEvent,
                context.Object,
                CancellationToken.None);

            // Assert
            logger.VerifyLog()
                .ErrorWasCalled()
                .MessageEquals("Error al publicar el evento de Outbox");

            context.Verify(
                c => c.SaveChangesAsync(CancellationToken.None),
                Times.Never);

            Assert.Multiple(
                () =>
                {
                    Assert.That(outboxEvent.IsProcessed, Is.False);
                    Assert.That(outboxEvent.ProcessedOn, Is.Null);
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
                        (MensajeRecibidoEvent)null!,
                        _jsonSettings),
                OccurredOn = DateTime.Now
            };

            var context = new Mock<DbContext>();
            var publisher = new Mock<IPublisher>();
            var logger = new Mock<ILogger<OutboxPublisher>>();
            var outboxPublisher = new OutboxPublisher(
                publisher.Object,
                logger.Object);

            // Act
            await outboxPublisher.PublishOutboxEventsAsync(
                outboxEvent,
                context.Object,
                CancellationToken.None);

            // Assert
            logger.VerifyLog()
                .ErrorWasCalled()
                .MessageEquals("Error al publicar el evento de Outbox");

            context.Verify(
                c => c.SaveChangesAsync(CancellationToken.None),
                Times.Never);

            Assert.Multiple(
                () =>
                {
                    Assert.That(outboxEvent.IsProcessed, Is.False);
                    Assert.That(outboxEvent.ProcessedOn, Is.Null);
                });
        }
    }
}
