using AppServices.Abstractions;
using Domain.Repositories;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.KernelPlugins
{
    internal class ConsultasPlugin(
        ITextEmbeddingGenerationService _textEmbeddingGenerationService,
        IConsultaRepository _consultaRepository,
        IRanker _ranker)
    {
        [KernelFunction("Buscar consultas")]
        [Description("Busca consultas simialres a la consulta actual")]
        public async Task<IEnumerable<string>> BuscarConsultasAsync(
            string consulta)
        {
            var embeddingConsulta = await _textEmbeddingGenerationService
                .GenerateEmbeddingAsync(consulta);

            var documents = await _consultaRepository
                .GetConsultasSimilaresAsync(embeddingConsulta);

            return documents.Select(d => d.ToString());
        }
    }
}
