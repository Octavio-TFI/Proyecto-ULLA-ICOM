using LangChain.Providers;
using LangChain.Providers.Ollama;
using Microsoft.Extensions.Configuration;
using System;

namespace SoporteLLM.Business.LlamaCpp
{
    public class LlamaCppEmbeddingModel(
        string url,
        LlamaCppOptions options) : Model<EmbeddingSettings>(
        options.ModelName), IEmbeddingModel
    {
        public LlamaCppProvider Provider
        {
            get;
        } = new LlamaCppProvider(url, options);

        public async Task<EmbeddingResponse> CreateEmbeddingsAsync(
            EmbeddingRequest request,
            EmbeddingSettings? settings = null,
            CancellationToken cancellationToken = default)
        {
            request = request ??
                throw new ArgumentNullException(nameof(request));

            if (request.Strings.Count < request.Images.Count)
            {
                throw new ArgumentException(
                    "Image count bigger than string count. One string should have 1 or 0 images",
                    nameof(request));
            }

            var results = new List<float[]>(capacity: request.Strings.Count);
            foreach (var prompt in request.Strings)
            {
                var images = Enumerable.Range(0, request.Images.Count)
                    .Select(
                        i => new ImageData
                        {
                            Id = i,
                            Data = request.Images[i]
                        })
                    .ToList();

                var response = await Provider.Api
                    .GenerateEmbeddingsAsync(
                        new LlamaCppEmbeddingRequest
                        {
                            Model = Id,
                            Content = prompt,
                            ImageData = images
                        })
                    .ConfigureAwait(false);

                results.Add(response.Embedding.Select(x => (float)x).ToArray());
            }

            return new EmbeddingResponse
            {
                Values = [.. results],
                UsedSettings = EmbeddingSettings.Default,
                Dimensions = results.FirstOrDefault()?.Length ?? 0,
            };
        }
    }
}
