using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Outbox.Abstractions
{
    internal interface IOutboxPublisher<Context> where Context : BaseContext
    {
        /// <summary>
        /// Publica un evento de outbox.
        /// </summary>
        /// <param name="outboxEvent">Evento de outbox</param>
        /// <param name="cancellationToken">Cancelation token</param>
        /// <returns></returns>
        Task PublishOutboxEventsAsync(
            OutboxEvent outboxEvent,
            CancellationToken cancellationToken);
    }
}
