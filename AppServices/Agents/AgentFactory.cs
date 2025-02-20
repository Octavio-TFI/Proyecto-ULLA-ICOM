using AppServices.Ports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Agents
{
    internal class AgentFactory(IFileManager fileManager)
    {
        readonly IFileManager _fileManager = fileManager;

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
                .GetRequiredService<IExecutionSettingsFactory>()
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
