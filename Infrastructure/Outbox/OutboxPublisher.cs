using Infrastructure.Outbox.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    internal class OutboxPublisher(
        IPublisher _publisher,
        ILogger<OutboxPublisher> _logger) : IOutboxPublisher
    {
        static readonly JsonSerializerSettings _jsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public async Task PublishOutboxEventsAsync(
            OutboxEvent outboxEvent,
            DbContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                var domainEvent = JsonConvert.DeserializeObject<INotification>(
                    outboxEvent.EventData,
                    _jsonSettings);

                if(domainEvent is not null)
                {
                    await _publisher.Publish(domainEvent, cancellationToken);
                }
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el evento de Outbox");

                return;
            }

            outboxEvent.IsProcessed = true;
            outboxEvent.ProcessedOn = DateTime.Now;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
