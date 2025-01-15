using AppServices.Ports;
using Infrastructure.Database;
using Infrastructure.Database.Chats;
using Infrastructure.Database.Embeddings;
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

            var chatContext = services.GetRequiredService<ChatContext>();
            var embeddingContext = services
                .GetRequiredService<EmbeddingContext>();

            var chatContextTask = ProcessContextEvents(
                chatContext,
                cancellationToken);

            var embeddingContextTask = ProcessContextEvents(
                embeddingContext,
                cancellationToken);

            await Task.WhenAll(chatContextTask, embeddingContextTask);
        }

        async Task ProcessContextEvents<Context>(
            Context context,
            CancellationToken cancellationToken)
            where Context : BaseContext
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
                        .GetRequiredService<IOutboxPublisher<Context>>();

                    await outboxPublisher.PublishOutboxEventsAsync(
                        outboxEvent,
                        cancellationToken);
                });
        }
    }
}
