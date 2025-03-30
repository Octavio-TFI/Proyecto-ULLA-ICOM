using AppServices.Abstractions;
using AppServices.Ports;
using Microsoft.Extensions.Configuration;
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
                (services, httpClient) =>
                {
                    string url = services.GetRequiredService<IConfiguration>()
                                .GetSection("Clients:Test:URL")
                                .Value ??
                        throw new Exception(
                                "Se debe configurar la URL de cliente de testing en Clients:Test:URL");

                    httpClient.BaseAddress = new Uri(url);
                });
            services.AddKeyedSingleton<IClient, TestClient>(Platforms.Test);

            return services;
        }
    }
}
