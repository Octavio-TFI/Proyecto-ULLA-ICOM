using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.DocumentLoaders;
using LangChain.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema.Generation;
using OpenAI;
using SoporteLLM.Abstractions;
using SoporteLLM.Model;
using System.Text.Json.Nodes;

namespace SoporteLLM.Business.CustomChains
{
    public class SummarizerChain : BaseStackableChain
    {
        readonly IChatModelFactory _modelFactory;

        public SummarizerChain(
            IChatModelFactory modelFactory,
            string inputKey = "docs",
            string outputKey = "summarized")
        {
            _modelFactory = modelFactory;
            InputKeys = [inputKey];
            OutputKeys = [outputKey];
        }


        protected override async Task<IChainValues> InternalCallAsync(
            IChainValues values,
            CancellationToken cancellationToken = default)
        {
            values = values ?? throw new ArgumentNullException(nameof(values));

            var documentsObject = values.Value[InputKeys[0]];
            if (documentsObject is not IEnumerable<Document> docs)
            {
                throw new ArgumentException(
                    $"{InputKeys[0]} is not a list of documents");
            }

            var schemaGenerator = new JSchemaGenerator();
            var schema = JsonNode.Parse(
                    schemaGenerator.Generate(typeof(SummarySchema)).ToString()) ??
                throw new Exception("Error creando JSON Schema");

            var summaryFunction = new Function(
                "Summarize",
                description: string.Empty,
                parameters: schema);

            var model = _modelFactory.Build([summaryFunction]);

            var summarizedDocs = new List<Document>();

            foreach (var doc in docs)
            {
                string descripcion = string.Empty;
                string preFixes = string.Empty;

                if (doc.Metadata.TryGetValue("Descripcion", out var des))
                {
                    descripcion = $"\nDescripcion: {des}";
                }

                if (doc.Metadata.TryGetValue("PreFixes", out var prefixes))
                {
                    preFixes = $"\n{prefixes}";
                }

                var system = new Message
                {
                    Role = MessageRole.System,
                    Content =
                        @"You are a support ticket summarizer of a software called Capataz. This ticket contains a problem with his title, description and fix.
The goal is to use this data to create a description of the problem and a text with steps to fix the problem. Provide this as a JSON with a two keys 'Description' 'Fix'."
                };

                var userMessage = new Message
                {
                    Role = MessageRole.Human,
                    Content =
                        @$"Here is the given support ticket:

Title: {doc.PageContent}{descripcion}{preFixes}
Fix: {doc.Metadata["Fix"]}"
                };

                var response = await model.GenerateAsync(
                    new ChatRequest { Messages = [system, userMessage] },
                    cancellationToken: cancellationToken);

                var summary = JsonConvert.DeserializeObject<SummarySchema>(
                    response.LastMessageContent);

                if (summary is not null)
                {
                    summarizedDocs.Add(new Document { PageContent = summary.Fix });
                }
            }

            values.Value[OutputKeys[0]] = summarizedDocs;
            return values;
        }
    }
}
