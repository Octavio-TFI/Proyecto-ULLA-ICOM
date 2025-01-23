using Infrastructure.LLM.DTOs;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM
{
    internal class LMStudioTextEmbeddingGenerationService(
        HttpClient _httpClient)
        : ITextEmbeddingGenerationService
    {
        public IReadOnlyDictionary<string, object?> Attributes
        {
            get;
        } = new Dictionary<string, object?>();

        public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(
            IList<string> data,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            var request = new EmbeddingRequest
            {
                Input = data,
                Model = "text-embedding-nomic-embed-text-v1.5@f16"
            };

            var response = await _httpClient.PostAsJsonAsync(
                string.Empty,
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error al obtener embeddings");
            }

            var embeddings = await response.Content
                    .ReadFromJsonAsync<EmbeddingResponseList>(cancellationToken) ??
                throw new Exception("Error al obtener embeddings");

            return embeddings.Data
                .Select(x => new ReadOnlyMemory<float>(x.Embedding))
                .ToList();
        }
    }
}
