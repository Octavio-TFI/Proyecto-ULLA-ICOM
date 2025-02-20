using Domain.Abstractions.Factories;
using Domain.Entities;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Factories
{
    internal class DocumentFactory(
        ITextEmbeddingGenerationService embeddingService)
        : IDocumentFactory
    {
        readonly ITextEmbeddingGenerationService _emmbeddingService = embeddingService;

        public async Task<Document> CreateAsync(
            string filename,
            string text,
            IList<string> textChunks)
        {
            var document = new Document { Filename = filename, Texto = text, };

            var chunkEmbeddings = await _emmbeddingService.GenerateEmbeddingsAsync(
                [.. textChunks]);

            for (int i = 0; i < textChunks.Count; i++)
            {
                var chunk = new DocumentChunk
                {
                    Texto = textChunks[i],
                    Embedding = chunkEmbeddings[i].ToArray(),
                };

                document.Chunks.Add(chunk);
            }

            return document;
        }
    }
}
