
using LangChain.Providers;
using OpenAI;
using SoporteLLM.Abstractions;
using SoporteLLM.Business.LlamaCpp;

namespace SoporteLLM.Business
{
    public class ChatModelFactory(IConfiguration config) : IChatModelFactory
    {
        readonly string _modelName = config["ChatModel"] ??
            throw new Exception("ChatModel no configurado");

        readonly string _url = config["ChatModelUrl"] ??
            throw new Exception("ChatModelUrl no configurada");

        public IChatModel Build(IEnumerable<Function> functions)
        {
            var model = CreateModel();

            model.AddGlobalTools(
                functions.Select(f => new Tool(f)).ToList(),
                new Dictionary<string, Func<string, CancellationToken, Task<string>>>(
                    ));

            return model;
        }

        public IChatModel Build()
        {
            return CreateModel();
        }

        LlamaCppChatModel CreateModel()
        {
            var model = new LlamaCppChatModel(
                _url,
                new LlamaCppOptions
                {
                    Temperature = 0,
                    Stop = ["<|eot_id|>"],
                    ModelName = _modelName
                });

            // Hooks to see internal processing
            model.PromptSent += (_, e) => Console.Write(e);
            model.PartialResponseGenerated += (_, e) => Console.Write(e);
            return model;
        }
    }
}
