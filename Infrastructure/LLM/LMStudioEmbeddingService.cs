using Domain.Abstractions;
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
    internal class LMStudioEmbeddingService(HttpClient _httpClient)
        : IEmbeddingService
    {
        public async Task<float[]> GenerateAsync(string texto)
        {
            var result = await GenerateAsync([texto]).ConfigureAwait(false);

            return result.First();
        }

        public async Task<List<float[]>> GenerateAsync(
            IList<string> textos)
        {
            var request = new EmbeddingRequest
            {
                Input = textos,
                Model = "text-embedding-nomic-embed-text-v1.5@f16"
            };

            var response = await _httpClient.PostAsJsonAsync(
                string.Empty,
                request)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error al obtener embeddings");
            }

            var embeddings = await response.Content
                    .ReadFromJsonAsync<EmbeddingResponseList>()
                    .ConfigureAwait(false) ??
                throw new Exception("Error al obtener embeddings");

            return embeddings.Data
                .Select(x => x.Embedding)
                .ToList();
        }
    }
}
