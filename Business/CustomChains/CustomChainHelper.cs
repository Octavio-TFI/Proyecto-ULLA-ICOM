using LangChain.Providers;
using SoporteLLM.Abstractions;

namespace SoporteLLM.Business.CustomChains
{
    public static class CustomChainHelper
    {
        public static RankingChain RankDocuments(
            IChatModelFactory modelFactory,
            string userKey = "user",
            string inputKey = "docs",
            string outputKey = "ranked")
        {
            return new RankingChain(modelFactory, userKey, inputKey, outputKey);
        }

        public static SummarizerChain SummarizeDocuments(
            IChatModelFactory modelFactory,
            string inputKey = "docs",
            string outputKey = "summarized")
        {
            return new SummarizerChain(modelFactory, inputKey, outputKey);
        }
    }
}
