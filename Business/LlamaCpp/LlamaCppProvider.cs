using LangChain.Providers;
using LangChain.Providers.Ollama;
using Microsoft.Extensions.Options;

namespace SoporteLLM.Business.LlamaCpp
{
    public class LlamaCppProvider(string url, LlamaCppOptions options)
        : Provider("LlamaCpp")
    {
        public LlamaCppApiClient Api { get; } = new(url);

        public LlamaCppOptions Options { get; } = options;
    }
}
