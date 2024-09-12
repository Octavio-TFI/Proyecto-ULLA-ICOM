using System.Text.Json.Serialization;

namespace SoporteLLM.Business.LlamaCpp
{
    public class LlamaCppEmbeddingResponse
    {
        [JsonPropertyName("embedding")]
        public double[] Embedding { get; set; } = [];
    }
}
