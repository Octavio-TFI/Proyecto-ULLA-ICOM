using Domain.Abstractions.Factories;
using Domain.Factories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public static class ServicesRegistration
    {
        public static void AddDomainServices(this IServiceCollection services)
        {
            services.AddSingleton<IMensajeFactory, MensajeFactory>();
        }
    }
}
