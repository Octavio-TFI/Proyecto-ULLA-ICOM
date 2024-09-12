using System.Text.Json.Serialization;

namespace SoporteLLM.Business.LlamaCpp
{
    public class ImageData
    {
        /// <summary>
        /// Id de la imagen para ser referenciada en el contenido del Embedding Request
        /// </summary>
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        /// <summary>
        /// Imagen codificada en un string Base64
        /// </summary>
        [JsonPropertyName("image_data")]
        public required string Data { get; set; }
    }
}
