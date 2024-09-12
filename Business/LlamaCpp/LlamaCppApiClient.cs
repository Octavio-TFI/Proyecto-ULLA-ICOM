using LangChain.Providers.Ollama;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoporteLLM.Business.LlamaCpp
{
    public class LlamaCppApiClient
    {
        readonly HttpClient _client;
        JsonSerializerOptions? _jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public LlamaCppApiClient(string url)
        {
            _client = new HttpClient { BaseAddress = new Uri(url) };
        }

        public async IAsyncEnumerable<LlamaCppChatResponse> GenerateCompletionAsync(
            LlamaCppChatRequest generateRequest)
        {
            var content = JsonSerializer.Serialize(
                generateRequest,
                _jsonSerializerOptions);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/completion");

            request.Content = new StringContent(
                content,
                Encoding.UTF8,
                "application/json");

            var completion = generateRequest.Stream
                ? HttpCompletionOption.ResponseHeadersRead
                : HttpCompletionOption.ResponseContentRead;

            using var response = await _client.SendAsync(request, completion)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content
                .ReadAsStreamAsync()
                .ConfigureAwait(false);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                string? line = await reader.ReadLineAsync().ConfigureAwait(false) ??
                    string.Empty;

                if (line == string.Empty)
                {
                    continue;
                }

                line = line.TrimStart("data: ".ToCharArray());

                var streamedResponse = JsonSerializer
                    .Deserialize<LlamaCppChatResponse>(line) ??
                    throw new InvalidOperationException(
                        "Response body was null");

                yield return streamedResponse;
            }
        }

        public async Task<LlamaCppEmbeddingResponse> GenerateEmbeddingsAsync(
            LlamaCppEmbeddingRequest generateRequest)
        {
            var content = JsonSerializer.Serialize(
                generateRequest,
                _jsonSerializerOptions);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/embeddings");

            request.Content = new StringContent(
                content,
                Encoding.UTF8,
                "application/json");

            var completion = HttpCompletionOption.ResponseContentRead;

            using var response = await _client.SendAsync(request, completion)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content
                .ReadAsStreamAsync()
                .ConfigureAwait(false);

            using var reader = new StreamReader(stream);
            string line = await reader.ReadToEndAsync().ConfigureAwait(false);

            var streamedResponse = JsonSerializer
                .Deserialize<LlamaCppEmbeddingResponse>(line) ??
                throw new InvalidOperationException("Response body was null");

            return streamedResponse;
        }
    }
}
