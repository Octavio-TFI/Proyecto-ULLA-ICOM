using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.VectorData;
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
    internal class DocumentacionPlugin(
        ITextEmbeddingGenerationService _textEmbeddingGenerationService,
        IDocumentRepository _documentRepository)
    {
        [KernelFunction("Buscar documentacion")]
        [Description("Busca documentación relevante a la consulta")]
        public async Task<IEnumerable<string>> BuscarDocumentacionAsync(
            string consulta)
        {
            var embeddingConsulta = await _textEmbeddingGenerationService
                .GenerateEmbeddingAsync(consulta);

            var documents = await _documentRepository
                .GetDocumentosRelacionadosAsync(embeddingConsulta);

            return documents.Select(d => d.ToString());
        }
    }
}
