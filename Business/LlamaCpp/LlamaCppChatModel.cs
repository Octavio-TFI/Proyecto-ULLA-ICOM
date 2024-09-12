using LangChain.Providers;
using OpenAI;
using System.Diagnostics;
using ChatRequest = LangChain.Providers.ChatRequest;
using ChatResponse = LangChain.Providers.ChatResponse;
using Message = LangChain.Providers.Message;

namespace SoporteLLM.Business.LlamaCpp
{
    public class LlamaCppChatModel(
        string url,
        LlamaCppOptions options) : ChatModel(options.ModelName)
    {
        public LlamaCppProvider Provider { get; } 
            = new LlamaCppProvider(url, options);

        protected List<Tool> GlobalTools { get; } = [];

        protected Dictionary<string, Func<string, CancellationToken, Task<string>>> Calls
        { get; } = [];

        public override async Task<ChatResponse> GenerateAsync(
            ChatRequest request,
            ChatSettings? settings = null,
            CancellationToken cancellationToken = default)
        {
            request = request ??
                throw new ArgumentNullException(nameof(request));

            var prompt = ToPrompt(request.Messages);
            var watch = Stopwatch.StartNew();
            var response = Provider.Api
                .GenerateCompletionAsync(
                    new LlamaCppChatRequest
                    {
                        Stream = true,
                        Prompt = prompt,
                        Stop = options.Stop,
                        Temperature = options.Temperature,
                        N_Predict = options.N_Predict,
                        JsonSchema =
                            GlobalTools.FirstOrDefault()?.Function.Parameters
                    });

            OnPromptSent(prompt);

            var buf = string.Empty;
            await foreach (var completion in response)
            {
                buf += completion.Content;
                OnPartialResponseGenerated(completion.Content);
            }

            OnCompletedResponseGenerated(buf);

            var result = request.Messages.ToList();
            var newMessage = ToMessage(buf);
            result.Add(newMessage);

            watch.Stop();

            var usage = LangChain.Providers.Usage.Empty with
            {
                Time = watch.Elapsed,
            };
            AddUsage(usage);
            Provider.AddUsage(usage);

            if (GlobalTools.Count > 0)
            {
            }

            return new ChatResponse
            {
                Messages = result,
                Usage = usage,
                UsedSettings = ChatSettings.Default,
            };
        }

        /// <summary>
        /// Añade tools que puede ejecutar el modelo en cada request.
        /// </summary>
        public void AddGlobalTools(
            ICollection<Tool> tools,
            IReadOnlyDictionary<string, Func<string, CancellationToken, Task<string>>> calls)
        {
            tools = tools ?? throw new ArgumentNullException(nameof(tools));
            calls = calls ?? throw new ArgumentNullException(nameof(calls));

            GlobalTools.AddRange(tools);
            foreach (var call in calls)
            {
                Calls.Add(call.Key, call.Value);
            }
        }

        public void ClearGlobalTools()
        {
            GlobalTools.Clear();
            Calls.Clear();
        }

        protected Message ToMessage(string buf)
        {
            if (GlobalTools.Count > 0)
            {
                //Por ahora solo puede llamar a una funcion
                return new Message
                {
                    Role = MessageRole.FunctionCall,
                    Content = buf,
                    FunctionName = GlobalTools.First().Function.Name
                };
            }
            else
            {
                return new Message { Role = MessageRole.Ai, Content = buf };
            }
        }

        protected string ConvertRole(MessageRole role)
        {
            return role switch
            {
                MessageRole.Human => options.HumanTag,
                MessageRole.Ai => options.AssistantTag,
                MessageRole.System => options.SystemTag,
                _ => throw new NotSupportedException(
                    $"the role {role} is not supported")
            };
        }

        protected string ConvertMessage(Message message)
        {
            if (message.Role == MessageRole.FunctionCall &&
                message.Role == MessageRole.FunctionResult)
            {
                return string.Empty;
            }

            return $"{ConvertRole(message.Role)}{message.Content}{options.EndMessageTag}";
        }

        protected string ToPrompt(IEnumerable<Message> messages)
        {
            return string.Join(
                string.Empty,
                messages.Select(ConvertMessage)
                    .Append(options.AssistantTag)
                    .ToArray());
        }

        protected virtual async Task CallFunctionsAsync(
            global::OpenAI.Chat.Message message,
            IList<Message> messages,
            CancellationToken cancellationToken = default)
        {
            message = message ??
                throw new ArgumentNullException(nameof(message));
            messages = messages ??
                throw new ArgumentNullException(nameof(messages));

            foreach (var tool in message.ToolCalls ?? [])
            {
                var func = Calls[tool.Function.Name];
                var json = await func(
                    tool.Function.Arguments.ToString(),
                    cancellationToken)
                    .ConfigureAwait(false);
                messages.Add(json.AsFunctionResultMessage(tool.Function.Name));
            }
        }
    }
}
