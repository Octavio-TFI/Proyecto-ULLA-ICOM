using LangChain.Databases;
using LangChain.DocumentLoaders;
using SoporteLLM.Abstractions;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace SoporteLLM.Business
{
    public class MesaDeAyudaDocumentSplitter : IDocumentSplitter
    {
        public ReadOnlyCollection<Document> SplitDocument(
            IReadOnlyCollection<Document> documents)
        {
            List<Document> docsSplitted = [];

            foreach (var doc in documents)
            {
                var document = XDocument.Parse(doc.PageContent);

                var defects = document.Descendants("Defect");

                foreach (var defect in defects)
                {
                    Dictionary<string, object> metadata = [];

                    string? id = defect
                        .Element("Id")?
                        .Value;

                    if (id is null)
                    {
                        continue;
                    }

                    metadata.Add("Id", id);

                    string? summary = defect
                        .Element("Summary")?
                        .Value;

                    if (summary is null)
                    {
                        continue;
                    }

                    string? fix = defect
                        .Element("Fix")?
                        .Value;

                    if (fix == null || fix == string.Empty)
                    {
                        continue;
                    }

                    metadata.Add("Fix", fix);

                    string? description = defect
                        .Element("Descripcion")?
                        .Value;

                    if (description != null && description != string.Empty)
                    {
                        metadata.Add("Descripcion", description);
                    }

                    string[] preFixes = defect.Elements("PreFix")
                        .Select(x => $"PreFix: {x.Value}")
                        .ToArray();

                    metadata.Add("PreFixes", string.Join("\n", preFixes));

                    docsSplitted.Add(
                        new Document
                        {
                            PageContent = summary,
                            Metadata = metadata
                        });
                }
            }

            return docsSplitted.AsReadOnly();
        }
    }
}
