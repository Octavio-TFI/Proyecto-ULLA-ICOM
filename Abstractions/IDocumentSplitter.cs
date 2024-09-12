using LangChain.Databases;
using LangChain.DocumentLoaders;
using System.Collections.ObjectModel;

namespace SoporteLLM.Abstractions
{
    public interface IDocumentSplitter
    {
        ReadOnlyCollection<Document> SplitDocument(
            IReadOnlyCollection<Document> documents);
    }
}
