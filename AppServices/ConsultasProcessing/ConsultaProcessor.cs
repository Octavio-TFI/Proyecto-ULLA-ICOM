using AppServices.Abstractions;
using AppServices.Agents;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Embeddings;
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

        public Task<Consulta> ProcessAsync(ConsultaData consultaData)
        {
            throw new NotImplementedException();
        }
    }
}
