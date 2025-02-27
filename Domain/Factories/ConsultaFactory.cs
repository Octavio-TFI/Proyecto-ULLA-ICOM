using Domain.Abstractions;
using Domain.Abstractions.Factories;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Factories
{
    internal class ConsultaFactory(
        [FromKeyedServices(TipoAgent.ProcesadorConsulta)] IAgent agent,
        IEmbeddingService embeddingService)
        : IConsultaFactory
    {
        readonly IAgent _agent = agent;
        readonly IEmbeddingService _embeddingService = embeddingService;

        public async Task<Consulta> CreateAsync(ConsultaData consultaData)
        {
            var agentResult = await _agent
                .GenerarRespuestaAsync(consultaData.ToString())
                .ConfigureAwait(false);

            var consultaResumen = JsonConvert.DeserializeObject<ConsultaResumen>(
                    agentResult.ToString()) ??
                throw new Exception("Error Deserializando Json ConsultaResumen");

            var embeddings = await _embeddingService
                .GenerateAsync(
                    [consultaData.Titulo, consultaResumen.Descripcion])
                .ConfigureAwait(false);

            return new Consulta
            {
                Id = consultaData.Id,
                Titulo = consultaData.Titulo,
                Descripcion = consultaResumen.Descripcion,
                Solucion = consultaResumen.Solucion,
                EmbeddingTitulo = embeddings[0].ToArray(),
                EmbeddingDescripcion = embeddings[1].ToArray()
            };
        }

        private class ConsultaResumen
        {
            public required string Descripcion { get; set; }

            public required string Solucion { get; set; }
        }
    }
}
