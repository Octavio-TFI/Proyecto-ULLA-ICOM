using Infrastructure.Outbox.Abstractions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    internal class OutboxService(IOutboxProcessor _outboxProcessor) : BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await _outboxProcessor.ProcessOutboxAsync(stoppingToken);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
