using Infrastructure.LLM.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.ExecutionSettingsFactories
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
            string? mimeType = null;

            if (schema is not null)
            {
                mimeType = "application/json";
            }

            // TODO: Ver de arreglar
            return new GeminiPromptExecutionSettings
            {
                FunctionChoiceBehavior = functionChoiceBehavior,
                ToolCallBehavior = GeminiToolCallBehavior.EnableKernelFunctions,
                ResponseSchema = schema,
                ResponseMimeType = mimeType,
                Temperature = temperature,
            };
        }
    }
}
