using AppServices.Abstractions;
using Domain.Abstractions;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Services
{
    internal class Ranker(
        [FromKeyedServices(TipoAgent.Ranker)] IAgent rankingAgent)
        : IRanker
    {
        readonly IAgent _rankingAgent = rankingAgent;

        public async Task<List<T>> RankAsync<T>(
            List<T> datosRecuperados,
            string consulta)
            where T : Entity
        {
            List<T> datosFiltrados = [];

            foreach (T datosRecuperado in datosRecuperados)
            {
                var arguments = new Dictionary<string, object?>
                {
                    ["document"] = datosRecuperado
                };

                var agentResult = await _rankingAgent
                    .GenerarRespuestaAsync(consulta, arguments)
                    .ConfigureAwait(false);

                var result = JsonConvert.DeserializeObject<RankerResult>(
                    agentResult);

                if (result?.Score is true)
                {
                    datosFiltrados.Add(datosRecuperado);
                }
            }


            return datosFiltrados;
        }
    }
}
