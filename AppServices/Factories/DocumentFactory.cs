using AppServices.Abstractions;
using Domain.Entities;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Factories
{
    internal class DocumentFactory(Kernel _kernel)
        : IDocumentFactory
    {
        ITextEmbeddingGenerationService _emmbeddingGenerator = _kernel
            .GetRequiredService<ITextEmbeddingGenerationService>();

        public async Task<Document> CreateAsync(
            string filename,
            string text,
            IEnumerable<Document> childs)
        {
            var embedding = await _emmbeddingGenerator.GenerateEmbeddingAsync(
                text);

            return new Document
            {
                Filename = filename,
                Texto = text,
                Embedding = embedding.ToArray(),
                Childs = childs.ToList()
            };
        }
    }
}
