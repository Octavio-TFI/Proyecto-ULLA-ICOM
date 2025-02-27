using AppServices.Ports;
using Domain.Abstractions;
using Domain.ValueObjects;
using Infrastructure.LLM.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM
{
    internal class AgentBuilder : IAgentBuilder
    {
        readonly IServiceProvider _serviceProvider;
        readonly IFileManager _fileManager;
        readonly IChatHistoryFactory _chatHistoryFactory;

        Type? Schema;
        FunctionChoiceBehavior? FunctionChoiceBehavior;
        readonly List<KernelPlugin> Tools = [];
        double? Temperature;

        internal AgentBuilder(
            IServiceProvider serviceProvider,
            IFileManager fileManager,
            IChatHistoryFactory chatHistoryFactory)
        {
            _fileManager = fileManager;
            _chatHistoryFactory = chatHistoryFactory;
        }

        public IAgentBuilder AddTool<T>(string name)
        {
            Tools.Add(
                KernelPluginFactory.CreateFromType<T>(
                    name,
                    _serviceProvider));

            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto();

            return this;
        }

        public IAgentBuilder SetResponseSchema(Type schema)
        {
            Schema = schema;

            return this;
        }

        public IAgentBuilder SetTemperature(double temperature)
        {
            Temperature = temperature;

            return this;
        }

        public IAgent Build(string name, TipoLLM tipoLLM)
        {
            string path = Path.Combine("Prompts", name, "Instructions.txt");

            string instructions = _fileManager.ReadAllText(path);

            var kernel = _serviceProvider.GetRequiredKeyedService<Kernel>(tipoLLM);

            kernel.Plugins.AddRange(Tools);

            var executionSettings = kernel
                .GetRequiredService<Abstractions.IExecutionSettingsFactory>()
                .Create(FunctionChoiceBehavior, Schema, Temperature);

            var chatAgent = new ChatCompletionAgent()
            {
                Name = name,
                Instructions = instructions,
                Kernel = kernel,
                Arguments = new KernelArguments(executionSettings)
            };

            return new Agent(chatAgent, _chatHistoryFactory);
        }

        public ChatCompletionAgent Create(
            Kernel kernel,
            string name,
            FunctionChoiceBehavior? functionChoiceBehavior = null,
            Type? schema = null,
            double? temperature = null)
        {
            string path = Path.Combine("Prompts", name, "Instructions.txt");

            string instructions = _fileManager.ReadAllText(path);

            var executionSettings = kernel
                .GetRequiredService<Abstractions.IExecutionSettingsFactory>()
                .Create(functionChoiceBehavior, schema, temperature);

            return new ChatCompletionAgent()
            {
                Name = name,
                Instructions = instructions,
                Kernel = kernel,
                Arguments = new KernelArguments(executionSettings)
            };
        }
    }
}
