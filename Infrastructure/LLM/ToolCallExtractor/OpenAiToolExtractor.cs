using Domain.ValueObjects;
using Infrastructure.LLM.Abstractions;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.ToolCallExtractor
{
    internal class OpenAiToolExtractor
        : IToolCallExtractor
    {
        public IEnumerable<AgentFunctionCall> Extract(
            ChatMessageContent chatMessage)
        {
            return FunctionCallContent.GetFunctionCalls(chatMessage)
                .Select(
                    f => new AgentFunctionCall
                    {
                        PluginName = f.PluginName,
                        FunctionName = f.FunctionName,
                        Arguments = f.Arguments
                    });
        }
    }
}
