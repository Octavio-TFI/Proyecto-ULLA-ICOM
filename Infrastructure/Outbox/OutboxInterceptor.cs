using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    internal class OutboxInterceptor
        : SaveChangesInterceptor
    {
        static readonly JsonSerializerSettings _jsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if(eventData.Context is not null)
            {
                await SaveOutboxMessagesAsync(eventData.Context);
            }

            return await base.SavingChangesAsync(
                eventData,
                result,
                cancellationToken);
        }

        static Task SaveOutboxMessagesAsync(DbContext context)
        {
            var ocurredOn = DateTime.Now;

            var outboxMessages = context.ChangeTracker
                .Entries<Entity>()
                .Where(entity => entity.State != EntityState.Unchanged)
                .SelectMany(
                    entity =>
                    {
                        return entity.Entity.Events
                            .Select(e => CreateOutboxEvent(e, ocurredOn));
                    })
                .ToList();

            return context.AddRangeAsync(outboxMessages);
        }

        static OutboxEvent CreateOutboxEvent(
            INotification @event,
            DateTime ocurredOn)
        {
            var type = @event.GetType().Name;
            var json = JsonConvert.SerializeObject(@event, _jsonSettings);

            return new OutboxEvent
            {
                EventType = type,
                EventData = json,
                OccurredOn = ocurredOn
            };
        }
    }
}
