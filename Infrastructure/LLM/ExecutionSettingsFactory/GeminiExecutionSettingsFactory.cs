using AppServices.Ports;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.ExecutionSettingsFactory
{
    internal class GeminiExecutionSettingsFactory
        : IExecutionSettingsFactory
    {
        public PromptExecutionSettings Create(
            FunctionChoiceBehavior? functionChoiceBehavior,
            Type? schema,
            double? temperature)
        {
            GeminiToolCallBehavior? geminiToolCallBehavior = null;

            if (functionChoiceBehavior is AutoFunctionChoiceBehavior)
            {
                geminiToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions;
            }
            else if (functionChoiceBehavior is RequiredFunctionChoiceBehavior)
            {
                geminiToolCallBehavior = GeminiToolCallBehavior.EnableKernelFunctions;
            }

            return new GeminiPromptExecutionSettings
            {
                FunctionChoiceBehavior = functionChoiceBehavior,
                ToolCallBehavior = geminiToolCallBehavior,
                ResponseSchema = schema,
                Temperature = temperature
            };
        }
    }
}
