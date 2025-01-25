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
            services.AddHttpClient(
                Platforms.Test,
                x => x.BaseAddress = new Uri("https://localhost:7128"));
            services.AddKeyedSingleton<IClient, TestClient>(Platforms.Test);

            return services;
        }
    }
}
