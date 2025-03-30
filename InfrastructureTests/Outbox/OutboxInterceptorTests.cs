using Domain.Entities;
using Domain.Events;
using Domain.ValueObjects;
using Infrastructure.Outbox;
using InfrastructureTests.Database.Tests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Outbox.Tests
{
    internal class OutboxInterceptorTests
    {
        [Test]
        public async Task SavingChangesAsync_WhenContextIsNotNull_ShouldSaveOutboxMessages(
            )
        {
            // Arrange
            List<INotification> domainEvents1 = [new MensajeRecibidoEvent
            {
                EntityId = Guid.NewGuid()
            }];
            List<INotification> domainEvents2 = [new MensajeRecibidoEvent
            {
                EntityId = Guid.NewGuid()
            }];

            var jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var event1Json = JsonConvert.SerializeObject(
                domainEvents1.First(),
                jsonSettings);
            var event2Json = JsonConvert.SerializeObject(
                domainEvents2.First(),
                jsonSettings);

            var entity1 = new Chat
            {
                ChatPlataformaId = "1",
                Plataforma = "1",
                UsuarioId = "Usuario1"
            };

            var entity2 = new Chat
            {
                ChatPlataformaId = "2",
                Plataforma = "2",
                UsuarioId = "Usuario2"
            };

            entity1.Events.AddRange(domainEvents1);
            entity2.Events.AddRange(domainEvents2);

            List<Entity> entities = [entity1, entity2];

            var eventDefinition = new EventDefinition(
                Mock.Of<ILoggingOptions>(),
                new EventId(),
                LogLevel.Trace,
                "1",
                (_) => (_, e) => e?.ToString());

            static string messageGenerator(EventDefinitionBase _, EventData __) => string.Empty;

            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            await context.AddRangeAsync(entities);

            var eventDataMock = new DbContextEventData(
                eventDefinition,
                messageGenerator,
                context);

            var expectedResult = InterceptionResult<int>.SuppressWithResult(10);

            var interceptor = new OutboxInterceptor();

            // Act
            var result = await interceptor.SavingChangesAsync(
                eventDataMock,
                expectedResult);

            // Assert
            await context.SaveChangesAsync();

            var datetimeNow = DateTime.Now;

            Assert.Multiple(
                () =>
                {
                    // Assert
                    Assert.That(
                        entities,
                        Has.All.Property(nameof(Entity.Events)).Empty);
                    Assert.That(context.OutboxEvents.Count(), Is.EqualTo(2));
                    Assert.That(
                        context.OutboxEvents,
                        Has.One.With
                                .Property(nameof(OutboxEvent.EventType))
                                .EqualTo("MensajeRecibidoEvent")
                                .With
                                .Property(nameof(OutboxEvent.IsProcessed))
                                .EqualTo(false)
                                .With
                                .Property(nameof(OutboxEvent.EventData))
                                .EqualTo(event1Json)
                                .With
                                .Property(nameof(OutboxEvent.ProcessedOn))
                                .EqualTo(null)
                                .With
                                .Property(nameof(OutboxEvent.OccurredOn))
                                .LessThanOrEqualTo(datetimeNow));
                    Assert.That(
                        context.OutboxEvents,
                        Has.One.With
                                .Property(nameof(OutboxEvent.EventType))
                                .EqualTo("MensajeRecibidoEvent")
                                .With
                                .Property(nameof(OutboxEvent.IsProcessed))
                                .EqualTo(false)
                                .With
                                .Property(nameof(OutboxEvent.EventData))
                                .EqualTo(event2Json)
                                .With
                                .Property(nameof(OutboxEvent.ProcessedOn))
                                .EqualTo(null)
                                .With
                                .Property(nameof(OutboxEvent.OccurredOn))
                                .LessThanOrEqualTo(datetimeNow));
                    Assert.That(result, Is.EqualTo(expectedResult));
                });
        }

        [Test]
        public async Task SavingChanges_WhenContextIsNull_ShouldDoNothing()
        {
            // Arrange
            var eventDefinition = new EventDefinition(
                Mock.Of<ILoggingOptions>(),
                new EventId(),
                LogLevel.Trace,
                "1",
                (_) => (_, e) => e?.ToString());

            Func<EventDefinitionBase, EventData, string> messageGenerator
                = (_, _) => string.Empty;

            var eventDataMock = new DbContextEventData(
                eventDefinition,
                messageGenerator,
                null);

            var expectedResult = InterceptionResult<int>.SuppressWithResult(10);
            var interceptor = new OutboxInterceptor();

            // Act
            var result = await interceptor.SavingChangesAsync(
                eventDataMock,
                expectedResult);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
