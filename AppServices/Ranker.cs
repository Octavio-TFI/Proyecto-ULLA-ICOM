using AppServices.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextGeneration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppServices
{
    internal class Ranker([FromKeyedServices(TipoKernel.Ranker)] Kernel _kernel)
        : IRanker
    {
        public async Task<List<T>> RankAsync<T>(
            List<T> datosRecuperados,
            string consulta)
            where T : Entity
        {
            List<T> datosFiltrados = [];

            foreach(T datosRecuperado in datosRecuperados)
            {
                var arguments = new KernelArguments()
                {
                    ["document"] = datosRecuperado.ToString(),
                    ["consulta"] = consulta
                };

                var functionResult = await _kernel.InvokeAsync(
                    "Ranker",
                    typeof(T).Name,
                    arguments);

                var result = JsonConvert.DeserializeObject<RankerResult>(
                    functionResult.ToString());

                if(result?.Score is true)
                {
                    datosFiltrados.Add(datosRecuperado);
                }
            }


            return datosFiltrados;
        }

        internal record RankerResult
        {
            public bool Score { get; set; }
        }
    }
}
