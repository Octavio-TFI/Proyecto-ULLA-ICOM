using AppServices.Abstractions;
using AppServices.Agents;
using Domain.Entities;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextGeneration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppServices.Ranking
{
    internal class Ranker(
        [FromKeyedServices(TipoAgent.Ranker)] ChatCompletionAgent rankingAgent)
        : IRanker
    {
        readonly ChatCompletionAgent _rankingAgent = rankingAgent;

        public async Task<List<T>> RankAsync<T>(
            List<T> datosRecuperados,
            string consulta)
            where T : Entity
        {
            List<T> datosFiltrados = [];

            foreach (T datosRecuperado in datosRecuperados)
            {
                var arguments = new KernelArguments()
                {
                    ["document"] = datosRecuperado.ToString()
                };

                ChatHistory chat = [];
                chat.AddUserMessage(consulta);

                var agentResult = await _rankingAgent
                    .InvokeAsync(chat, arguments)
                    .FirstAsync();

                var result = JsonConvert.DeserializeObject<RankerResult>(
                    agentResult.ToString());

                if (result?.Score is true)
                {
                    datosFiltrados.Add(datosRecuperado);
                }
            }


            return datosFiltrados;
        }
    }
}
