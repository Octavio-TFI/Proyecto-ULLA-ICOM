using AppServices.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    public static class ServicesRegistration
    {
        /// <summary>
        /// Registra los servicios de la capa de aplicación
        /// </summary>
        /// <param name="services">Collección de servicios</param>
        /// <returns>Collección de servicios que incluye los servicios de la capa de aplicación</returns>
        public static IServiceCollection AddAppServices(
            this IServiceCollection services)
        {
            services.AddMediatR(
                c => c.RegisterServicesFromAssembly(
                    Assembly.GetExecutingAssembly()));

            return services.AddScoped<IRecibidorMensajes, RecibidorMensajes>();
        }
    }
}
