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
    internal class OutboxProcessor(
        IServiceScopeFactory _serviceScopeFactory,
        IOutboxPublisher _outboxPublisher) : IOutboxProcessor
    {
        public async Task ProcessOutboxAsync(
            CancellationToken cancellationToken)
        {
            var services = _serviceScopeFactory.CreateScope().ServiceProvider;

            var chatContext = services.GetRequiredService<ChatContext>();
            var embeddingContext = services
                .GetRequiredService<EmbeddingContext>();

            var chatContextTask = ProcessContextEvents(
                _outboxPublisher,
                chatContext,
                cancellationToken);

            var embeddingContextTask = ProcessContextEvents(
                _outboxPublisher,
                embeddingContext,
                cancellationToken);

            await Task.WhenAll(chatContextTask, embeddingContextTask);
        }

        private static async Task ProcessContextEvents(
            IOutboxPublisher _outboxPublisher,
            BaseContext chatContext,
            CancellationToken cancellationToken)
        {
            var outboxEvents = await chatContext.OutboxEvents
                .Where(x => x.IsProcessed == false)
                .OrderBy(x => x.OccurredOn)
                .Take(100)
                .ToListAsync(cancellationToken);

            await Parallel.ForEachAsync(
                outboxEvents,
                async (outboxEvent, CancellationToken) => await _outboxPublisher
                    .PublishOutboxEventsAsync(
                        outboxEvent,
                        chatContext,
                        cancellationToken));
        }
    }
}
