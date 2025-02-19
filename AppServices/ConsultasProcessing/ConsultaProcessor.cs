using AppServices.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.ConsultasProcessing
{
    internal class ConsultaProcessor(
        [FromKeyedServices(TipoKernel.ConsultasProcessing)] Kernel kernel)
        : IConsultaProcessor
    {
        readonly Kernel _kernel = kernel;
        readonly ITextEmbeddingGenerationService _embeddingService = kernel
            .GetRequiredService<ITextEmbeddingGenerationService>();

        public Task<Consulta> ProcessAsync(ConsultaData consultaData)
        {
            throw new NotImplementedException();
        }
    }
}
