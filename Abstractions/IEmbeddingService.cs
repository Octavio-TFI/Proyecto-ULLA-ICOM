using LangChain.Splitters.Text;

namespace SoporteLLM.Abstractions
{
    public interface IEmbeddingService
    {
        public Task CreateEmbeddingsAsync();
    }
}
