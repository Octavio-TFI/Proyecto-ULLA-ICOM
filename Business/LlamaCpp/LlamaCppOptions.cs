using System.Text.Json.Serialization;

namespace SoporteLLM.Business.LlamaCpp
{
    public class LlamaCppOptions
    {
        /// <summary>
        /// Nomnre del LLM
        /// </summary>
        public required string ModelName { get; set; }

        /// <summary>
        /// Especifica el comienzo del mensaje de sistema. Por defecto Llama3.
        /// Por ejemplo, System:
        /// </summary>
        public string SystemTag
        { get;
            set; } = "<|start_header_id|>system<|end_header_id|>\n\n";


        /// <summary>
        /// Especifica el comienzo del mensaje del humano. Por defecto Llama3.
        /// Por ejemplo, Human:
        /// </summary>
        public string HumanTag
        { get;
            set; } = "<|start_header_id|>user<|end_header_id|>\n\n";


        /// <summary>
        /// Especifica el comienzo del mensaje de asistente. Por defecto Llama3.
        /// Por ejemplo, Assistant:
        /// </summary>
        public string AssistantTag
        { get;
            set; } = "<|start_header_id|>assistant<|end_header_id|>\n\n";

        /// <summary>
        /// Especifica el final de un mensaje. Por defecto Llama3.
        /// </summary>
        public string EndMessageTag
        { get;
            set; } = "<|eot_id|>\n";


        /// <summary>
        /// Array de tokens de stop. El modelo para de generar cuando se
        /// encuantra con estos tokens.
        /// </summary>
        public string[] Stop { get; set; } = [];

        /// <summary>
        /// Cantidad maxima de tokens a generar, por defecto -1(infinito).
        /// </summary>
        public int N_Predict { get; set; } = -1;

        /// <summary>
        /// Ajusta la aleatoriedad del texto generado. Si es igual a 0, el texto
        /// es "exacto". Si es 1, es lo "creativo" posible.
        /// </summary>
        public int Temperature { get; internal set; }
    }
}
