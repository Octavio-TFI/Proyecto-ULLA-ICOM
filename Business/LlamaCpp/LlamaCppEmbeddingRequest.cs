using LangChain.Providers.LLamaSharp;
using System.Text.Json.Serialization;

namespace SoporteLLM.Business.LlamaCpp
{
    public class LlamaCppEmbeddingRequest
    {
        /// <summary>
        /// Modelo a utilizar para generar Embeddings
        /// </summary>
        [JsonPropertyName("model")]
        public required string Model { get; set; }

        /// <summary>
        /// Texto a procesar
        /// </summary>
        [JsonPropertyName("content")]
        public required string Content { get; set; }

        /// <summary>
        /// Array de objetos que tienen imagenes codificadas en Base64 con sus ids
        /// Para ser referenciadas en el contenido del request.
        /// Ejemplo: Image: [img-21].\nCaption: This is a picture of a house.
        /// [img-21] sera reemplazado por los embeddings de la imagen
        /// Response: {..., "image_data": [{"data": "<BASE64_STRING>", "id": 21}]}
        /// </summary>
        [JsonPropertyName("image_data")]
        public ICollection<ImageData> ImageData { get; set; } = [];
    }
}
