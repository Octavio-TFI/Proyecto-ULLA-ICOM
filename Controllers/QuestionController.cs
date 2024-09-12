using LangChain.Databases;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Providers.LLamaSharp;
using LangChain.Schema;
using Microsoft.AspNetCore.Mvc;
using SoporteLLM.Abstractions;
using static SoporteLLM.Business.CustomChains.CustomChainHelper;
using static LangChain.Chains.Chain;

namespace SoporteLLM.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class QuestionController(
        IChatModelFactory modelFactory,
        IEmbeddingModel embeddingModel,
        IVectorDatabase vectorDatabase) : ControllerBase
    {
        const string collectionName = "MesaDeAyuda";

        const string userKey = "user";
        const string docsKey = "docs";
        const string rankedDocsKey = "ranked";
        const string summarizedDocsKey = "summarized";
        const string contextKey = "context";

        readonly IChatModelFactory _modelFactory = modelFactory;
        readonly IEmbeddingModel _embeddingModel = embeddingModel;
        readonly IVectorDatabase _vectorDatabase = vectorDatabase;

        [HttpGet]
        public async Task<string?> Get(string message)
        {
            var template = @$"<|begin_of_text|>
<|start_header_id|>system<|end_header_id|>
Eres un asistente de atención al cliente de un software llamado Capataz. Debes enumerar distintas soluciones concisas al cliente en español sobre como solucionar el problema. Utiliza la información a continuación que contiene problemas pasados. No debes mostrar datos personales ni nombres de personas o empresas. Si la respuesta no esta en la información o la solucion fue una conexión remota digale al cliente que se comunique con mesa de ayuda, no trates de dar una respuesta.
{{{contextKey}}}
<|eot_id|>
<|start_header_id|>user<|end_header_id|>
{{{userKey}}}
<|eot_id|>
<|start_header_id|>assistant<|end_header_id|>";

            var vectorCollection = await _vectorDatabase.GetCollectionAsync(
                collectionName);

            var chain =
                Set(message, outputKey: userKey) |
                RetrieveDocuments(
                    vectorCollection,
                    _embeddingModel,
                    10,
                    inputKey: userKey,
                    outputKey: docsKey) |
                SummarizeDocuments(
                    _modelFactory,
                    inputKey: docsKey,
                    outputKey: summarizedDocsKey) |
                RankDocuments(
                    _modelFactory,
                    userKey: userKey,
                    inputKey: summarizedDocsKey,
                    outputKey: rankedDocsKey) |
                StuffDocuments(inputKey: rankedDocsKey, outputKey: contextKey) |
                Template(template) |
                LLM(_modelFactory.Build());

            var response = await chain.RunAsync();

            return response.Value.Values.Last().ToString();
        }
    }
}
