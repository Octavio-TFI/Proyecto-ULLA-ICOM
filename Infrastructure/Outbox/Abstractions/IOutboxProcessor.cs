using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Outbox.Abstractions
{
    internal interface IOutboxProcessor
    {
        /// <summary>
        /// Procesa los eventos de Outbox.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        Task ProcessOutboxAsync(CancellationToken cancellationToken);
    }
}
