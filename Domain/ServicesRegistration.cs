using AppServices.Abstractions;
using Domain.Abstractions;
using Domain.Abstractions.Factories;
using Domain.Entities;
using Domain.Factories;
using Domain.Repositories;
using Domain.Services;
using Domain.ValueObjects;
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
            services.AddSingleton<IConsultaFactory, ConsultaFactory>();
            services.AddSingleton<IDocumentFactory, DocumentFactory>();
            services.AddSingleton<IRanker, Ranker>();

            services.AddKeyedTransient(
                TipoAgent.Ranker,
                (services, key) =>
                {
                    var builder = services.GetRequiredService<IAgentBuilder>();

                    return builder
                        .SetTemperature(0.2)
                        .SetResponseSchema(typeof(RankerResult))
                        .Build(TipoAgent.Ranker, TipoLLM.Pequeño);
                });

            services.AddKeyedTransient(
                TipoAgent.ProcesadorConsulta,
                (services, key) =>
                {
                    var builder = services.GetRequiredService<IAgentBuilder>();

                    return builder
                        .SetTemperature(0.2)
                        .SetResponseSchema(typeof(ConsultaResumen))
                        .Build(TipoAgent.ProcesadorConsulta, TipoLLM.Grande);
                });

            services.AddKeyedTransient(
                TipoAgent.Chat,
                (services, key) =>
                {
                    var builder = services.GetRequiredService<IAgentBuilder>();

                    return builder
                        .AddTool<InformacionTool>("buscar")
                        .SetTemperature(0.2)
                        .Build(TipoAgent.Chat, TipoLLM.Grande);
                });
        }
    }
}
