using AppServices.Ports;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.ExecutionSettingsFactory
{
    internal class OpenAiExecutionSettingsFactory
        : IExecutionSettingsFactory
    {
        public PromptExecutionSettings Create(
            FunctionChoiceBehavior? functionChoiceBehavior,
            Type? schema,
            double? temperature)
        {
            return new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = functionChoiceBehavior,
                ResponseFormat = schema,
                Temperature = temperature
            };
        }
    }
}
