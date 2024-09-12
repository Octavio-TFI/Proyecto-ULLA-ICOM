using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SoporteLLM.Business.LlamaCpp
{
    public class LlamaCppChatRequest
    {
        /// <summary>
        /// Si es false, la response sera devuelta como un solo objeto. Si es
        /// true, como un stream de objetos.
        /// </summary>
        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = true;

        /// <summary>
        /// Promt a enviar
        /// </summary>
        [JsonPropertyName("prompt")]
        public required string Prompt { get; set; }

        /// <summary>
        /// Cantidad maxima de tokens a generar, por defecto -1(infinito).
        /// </summary>
        [JsonPropertyName("n_predict")]
        public int N_Predict { get; set; } = -1;

        /// <summary>
        /// Ajusta la aleatoriedad del texto generado. Si es igual a 0, el texto
        /// es "exacto". Si es 1, es lo "creativo" posible.
        /// </summary>
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.8;

        /// <summary>
        /// Array de tokens de stop. El modelo para de generar cuando se
        /// encuantra con estos tokens.
        /// </summary>
        [JsonPropertyName("stop")]
        public string[] Stop { get; set; } = [];

        [JsonPropertyName("json_schema")]
        public JsonNode? JsonSchema { get; set; } = null;
    }
}
