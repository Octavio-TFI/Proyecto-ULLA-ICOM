using AppServices.Ports;
using Infrastructure.Database;
using Infrastructure.Outbox.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    internal class OutboxProcessor(IServiceScopeFactory _serviceScopeFactory) : IOutboxProcessor
    {
        static readonly JsonSerializerSettings _jsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public async Task ProcessOutboxAsync(
            CancellationToken cancellationToken)
        {
            var services = _serviceScopeFactory.CreateScope().ServiceProvider;

            var context = services.GetRequiredService<ChatContext>();
            var publisher = services.GetRequiredService<IPublisher>();

            var outboxEvents = await context.OutboxEvents
                .Where(x => x.IsProcessed == false)
                .OrderBy(x => x.OccurredOn)
                .Take(100)
                .ToListAsync(cancellationToken);

            await Parallel.ForEachAsync(
                outboxEvents,
                async (outboxEvent, CancellationToken) => await ProcessEventAsync(
                    publisher,
                    outboxEvent,
                    cancellationToken));
        }

        private static async Task ProcessEventAsync(
            IPublisher publisher,
            OutboxEvent outboxEvent,
            CancellationToken cancellationToken)
        {
            try
            {
                var domainEvent = JsonConvert.DeserializeObject<INotification>(
                    outboxEvent.EventData,
                    _jsonSettings);

                if(domainEvent is not null)
                {
                    await publisher.Publish(domainEvent, cancellationToken);
                }
            } catch(Exception ex)
            {
            }
        }
    }
}
