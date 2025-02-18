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
            IList<string> textChunks)
        {
            var document = new Document { Filename = filename, Texto = text, };

            var chunkEmbeddings = await _emmbeddingGenerator.GenerateEmbeddingsAsync(
                textChunks.ToList());

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
