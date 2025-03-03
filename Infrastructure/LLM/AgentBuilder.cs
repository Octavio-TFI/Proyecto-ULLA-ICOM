using AppServices.Ports;
using Domain.Abstractions;
using Domain.ValueObjects;
using Infrastructure.LLM.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM
{
    internal class AgentBuilder(
        IServiceProvider serviceProvider,
        IFileManager fileManager,
        IChatHistoryAdapter chatHistoryFactory)
        : IAgentBuilder
    {
        readonly IServiceProvider _serviceProvider = serviceProvider;
        readonly IFileManager _fileManager = fileManager;
        readonly IChatHistoryAdapter _chatHistoryFactory = chatHistoryFactory;

        readonly AgentData AgentData = new();

        public Type? Schema { get; private set; }

        public FunctionChoiceBehavior? FunctionChoiceBehavior { get; private set; }

        public List<KernelPlugin> Tools { get; } = [];

        public double? Temperature { get; private set; }

        public IAgentBuilder AddTool<T>(string name)
        {
            var methods = typeof(T).GetMethods(
                BindingFlags.Instance | BindingFlags.Public);

            List<KernelFunction> functions = [];

            foreach (var method in methods)
            {
                string? functionName = method
                    .GetCustomAttribute<DisplayNameAttribute>()?
                    .DisplayName;

                if (functionName is null)
                {
                    continue;
                }

                string? functionDescription = method
                    .GetCustomAttribute<DescriptionAttribute>()?
                    .Description;

                var parameters = method.GetParameters()
                    .Where(
                        p => p.GetCustomAttribute<DescriptionAttribute>() is not null)
                    .Select(
                        p => new KernelParameterMetadata(p.Name!)
                        {
                            Description =
                                p.GetCustomAttribute<DescriptionAttribute>()?.Description,
                        });

                functions.Add(
                    KernelFunctionFactory.CreateFromMethod(
                        method,
                        ActivatorUtilities.CreateInstance<T>(
                            _serviceProvider,
                            AgentData),
                        new KernelFunctionFromMethodOptions
                    {
                        FunctionName = functionName,
                        Description = functionDescription,
                        Parameters = parameters,
                        ReturnParameter =
                            new KernelReturnParameterMetadata
                                    {
                                        ParameterType = method.ReturnType
                                    },
                    }));
            }

            Tools.Add(KernelPluginFactory.CreateFromFunctions(name, functions));

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

            var kernel = _serviceProvider.GetRequiredKeyedService<Kernel>(
                tipoLLM);

            kernel.Plugins.AddRange(Tools);

            var executionSettings = kernel
                .GetRequiredService<IExecutionSettingsFactory>()
                .Create(FunctionChoiceBehavior, Schema, Temperature);

            var chatAgent = new ChatCompletionAgent()
            {
                Name = name,
                Instructions = instructions,
                Kernel = kernel,
                Arguments = new KernelArguments(executionSettings)
            };

            return new Agent(chatAgent, AgentData, _chatHistoryFactory);
        }
    }
}
