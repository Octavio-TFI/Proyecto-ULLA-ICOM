﻿using Infrastructure.Database;
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
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    internal class OutboxPublisher(
        IPublisher _publisher,
        ILogger<OutboxPublisher> _logger,
        ChatContext _context) : IOutboxPublisher
    {
        static readonly JsonSerializerSettings _jsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public async Task PublishOutboxEventsAsync(
            OutboxEvent outboxEvent,
            CancellationToken cancellationToken)
        {
            try
            {
                var domainEvent = JsonConvert.DeserializeObject<INotification>(
                        outboxEvent.EventData,
                        _jsonSettings) ??
                    throw new NullReferenceException(
                        "Deserializacion de domain event devolvio null");

                await _publisher.Publish(domainEvent, cancellationToken);
            } catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Error al publicar el evento de Outbox de tipo {outboxEvent.EventType}");

                return;
            }

            outboxEvent.IsProcessed = true;
            outboxEvent.ProcessedOn = DateTime.Now;

            _context.Update(outboxEvent);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
