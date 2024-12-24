using Infrastructure.Outbox.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    public static class OutboxServicesRegistration
    {
        public static IServiceCollection AddOutboxServices(
            this IServiceCollection services)
        {
            services.AddHostedService<OutboxService>();
            services.AddSingleton<IOutboxProcessor, OutboxProcessor>();
            services.AddSingleton<IOutboxPublisher, OutboxPublisher>();

            return services;
        }
    }
}
