using LangChain.Databases;
using LangChain.Databases.Sqlite;
using LangChain.DocumentLoaders;
using LangChain.Providers;
using OpenAI.Constants;

namespace SoporteLLM.Services
{
    public static class VectorDatabaseServiceHelper
    {
        public static IServiceCollection AddVectorDatabase(
            this IServiceCollection services,
            string databaseFile)
        {
            var vectorDb = new SqLiteVectorDatabase(databaseFile);

            return services.AddSingleton<IVectorDatabase>(vectorDb);
        }
    }
}
