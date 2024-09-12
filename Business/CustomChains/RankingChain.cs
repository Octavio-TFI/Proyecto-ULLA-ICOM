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
    public class RankingChain : BaseStackableChain
    {
        readonly IChatModelFactory _modelFactory;

        public RankingChain(
            IChatModelFactory modelFactory,
            string userKey = "user",
            string inputKey = "docs",
            string outputKey = "ranked")
        {
            _modelFactory = modelFactory;
            InputKeys = [userKey,inputKey];
            OutputKeys = [outputKey];
        }


        protected override async Task<IChainValues> InternalCallAsync(
            IChainValues values,
            CancellationToken cancellationToken = default)
        {
            values = values ?? throw new ArgumentNullException(nameof(values));

            var documentsObject = values.Value[InputKeys[1]];
            if (documentsObject is not IEnumerable<Document> docs)
            {
                throw new ArgumentException(
                    $"{InputKeys[0]} is not a list of documents");
            }

            var schemaGenerator = new JSchemaGenerator();
            var schema = JsonNode.Parse(
                    schemaGenerator.Generate(typeof(RankingSchema)).ToString()) ??
                throw new Exception("Error creando JSON Schema");

            var rankFunction = new Function(
                "Rank",
                description: string.Empty,
                parameters: schema);

            var model = _modelFactory.Build([rankFunction]);

            var rankedDocs = new List<Document>();

            foreach (var doc in docs)
            {
                var system = new Message
                {
                    Role = MessageRole.System,
                    Content =
                        @"You are a grader assessing relevance of a retrieved document to a user question. 
If the document contains keywords related to the user question, grade it as relevant. It does not need to be a stringent test. The goal is to filter out erroneous retrievals.
Give a binary score 'true' or 'false' score to indicate whether the document is relevant to the question.
Provide the binary score as a JSON with a single key 'Score' and no premable or explanation."
                };

                var userMessage = new Message
                {
                    Role = MessageRole.Human,
                    Content =
                        @$"Here is the retrieved document:

{doc.PageContent}

Here is the user question: {values.Value[InputKeys[0]]}"
                };

                var response = await model.GenerateAsync(
                    new ChatRequest { Messages = [system,userMessage] },
                    cancellationToken: cancellationToken);

                var rank = JsonConvert.DeserializeObject<RankingSchema>(
                    response.LastMessageContent);

                if (rank?.Score == true)
                {
                    rankedDocs.Add(doc);
                }
            }

            values.Value[OutputKeys[0]] = rankedDocs;
            return values;
        }
    }
}
