using LangChain.Providers;
using LangChain.Providers.LLamaSharp;
using SoporteLLM.Business.LlamaCpp;

namespace SoporteLLM.Services
{
    public static class EmbeddingModelServiceHelper
    {
        public static IServiceCollection AddEmbeddingModel(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            string modelName = configuration["EmbeddingsModel"] ??
                throw new Exception("EmbeddingModel no configurado");

            string url = configuration["EmbeddingsModelUrl"] ??
                throw new Exception("EmbeddingsModelUrl no configurada");

            var model = new LlamaCppEmbeddingModel(
                url,
                new LlamaCppOptions
                {
                    ModelName = modelName
                });

            return services
                .AddSingleton<IEmbeddingModel>(model);
        }
    }
}
