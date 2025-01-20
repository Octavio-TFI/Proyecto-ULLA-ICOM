using AppServices.Abstractions;
using AppServices.Ports;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Clients
{
    public static class ClientsServicesRegistration
    {
        public static IServiceCollection AddClientServices(
            this IServiceCollection services)
        {
            services.AddKeyedSingleton<IClient, TestClient>(Platforms.Test);

            return services;
        }
    }
}
