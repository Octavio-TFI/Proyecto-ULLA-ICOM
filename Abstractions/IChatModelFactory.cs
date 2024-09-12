using LangChain.Providers;
using OpenAI;

namespace SoporteLLM.Abstractions
{
    public interface IChatModelFactory
    {
        /// <summary>
        /// Construye un modelo sin functions calls
        /// </summary>
        /// <returns>Chat model</returns>
        IChatModel Build();

        /// <summary>
        /// Construye un podelo que puede utilizar functions calls
        /// </summary>
        /// <param name="functions">Las functions que puede llamar el modelo</param>
        /// <returns>Chat model</returns>
        IChatModel Build(IEnumerable<Function> functions);
    }
}
