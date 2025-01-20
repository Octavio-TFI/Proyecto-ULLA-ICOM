using AppServices.Abstractions;
using AppServices.KernelPlugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
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
            services.AddSingleton<IRanker, Ranker>();
            services.AddSingleton<IChatHistoryFactory, ChatHistoryFactory>();
            services.AddScoped<IGeneradorRespuesta, GeneradorRespuesta>();

            services.AddKeyedTransient(
                TipoKernel.Ranker,
                (services, key) =>
                {
                    var kernel = services.GetRequiredService<Kernel>();

                    var rankerPromptsDirectory = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Prompts",
                        "Ranker");

                    var rankerPlugin = kernel.ImportPluginFromPromptDirectory(
                        rankerPromptsDirectory);

                    return kernel;
                });

            services.AddKeyedScoped(
                TipoKernel.GeneradorRepuestas,
                (services, key) =>
                {
                    var kernel = services.GetRequiredService<Kernel>();

                    kernel.Plugins
                        .AddFromType<InformacionPlugin>(
                            serviceProvider: services);

                    return kernel;
                });

            services.AddMediatR(
                c => c.RegisterServicesFromAssembly(
                    Assembly.GetExecutingAssembly()));

            return services.AddScoped<IRecibidorMensajes, RecibidorMensajes>();
        }
    }
}
