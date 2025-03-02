using Infrastructure.Database;
using Infrastructure.Outbox.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    internal class OutboxProcessor(IServiceScopeFactory _serviceScopeFactory)
        : IOutboxProcessor
    {
        public async Task ProcessOutboxAsync(
            CancellationToken cancellationToken)
        {
            var services = _serviceScopeFactory.CreateScope().ServiceProvider;

            var context = services.GetRequiredService<ChatContext>();

            var chatContextTask = ProcessContextEvents(
                context,
                cancellationToken);

            await Task.WhenAll(chatContextTask);
        }

        async Task ProcessContextEvents(
            ChatContext context,
            CancellationToken cancellationToken)
        {
            var outboxEvents = await context.OutboxEvents
                .Where(x => x.IsProcessed == false)
                .OrderBy(x => x.OccurredOn)
                .Take(100)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            await Parallel.ForEachAsync(
                outboxEvents,
                async (outboxEvent, CancellationToken) =>
                {
                    var outboxPublisher = _serviceScopeFactory.CreateScope()
                        .ServiceProvider
                        .GetRequiredService<IOutboxPublisher>();

                    await outboxPublisher.PublishOutboxEventsAsync(
                        outboxEvent,
                        cancellationToken);
                });
        }
    }
}
