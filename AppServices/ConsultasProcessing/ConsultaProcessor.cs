using AppServices.Abstractions;
using AppServices.Agents;
using AppServices.Ranking;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.ConsultasProcessing
{
    internal class ConsultaProcessor(
        [FromKeyedServices(TipoAgent.ProcesadorConsulta)] ChatCompletionAgent agent,
        ITextEmbeddingGenerationService embeddingService)
        : IConsultaProcessor
    {
        readonly ChatCompletionAgent _agent = agent;
        readonly ITextEmbeddingGenerationService _embeddingService = embeddingService;

        public async Task<Consulta> ProcessAsync(ConsultaData consultaData)
        {
            ChatHistory chat = [];
            chat.AddUserMessage(consultaData.ToString());

            var agentResult = await _agent
                .InvokeAsync(chat)
                .FirstAsync();

            var consultaResumen = JsonConvert.DeserializeObject<ConsultaResumen>(
                    agentResult.ToString()) ??
                throw new Exception("Error Deserializando Json ConsultaResumen");

            var embeddings = await _embeddingService.GenerateEmbeddingsAsync(
                [consultaData.Titulo, consultaResumen.Descripcion]);

            return new Consulta
            {
                Titulo = consultaData.Titulo,
                Descripcion = consultaResumen.Descripcion,
                Solucion = consultaResumen.Solucion,
                EmbeddingTitulo = embeddings[0].ToArray(),
                EmbeddingDescripcion = embeddings[1].ToArray()
            };
        }
    }
}
