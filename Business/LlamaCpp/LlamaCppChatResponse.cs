using System.Text.Json.Serialization;

namespace SoporteLLM.Business.LlamaCpp
{
    public class LlamaCppChatResponse
    {
        /// <summary>
        /// Resultado de la generacion. En caso de modo streaming, contiene el proximo token de la generación.
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la generación paró.
        /// </summary>
        [JsonPropertyName("stop")]
        public bool Stop { get; set; }
    }
}
