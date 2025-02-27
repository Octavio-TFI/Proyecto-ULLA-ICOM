using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.Abstractions
{
    internal interface IExecutionSettingsFactory
    {
        PromptExecutionSettings Create(
            FunctionChoiceBehavior? functionChoiceBehavior,
            Type? schema,
            double? temperature);
    }
}
