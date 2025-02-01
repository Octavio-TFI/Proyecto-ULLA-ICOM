using AppServices.Abstractions;
using Domain.Entities;
using Microsoft.SemanticKernel.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Factories
{
    internal class DocumentFactory(
        ITextEmbeddingGenerationService _embeddingGenerator)
        : IDocumentFactory
    {
        public async Task<Document> CreateAsync(
            string filename,
            string text,
            IEnumerable<Document> childs)
        {
            var embedding = await _embeddingGenerator
                .GenerateEmbeddingAsync(text);

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
