using Domain.ValueObjects;
using Infrastructure.LLM.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.ToolCallExtractor
{
    internal class GeminiToolCallExtractor
        : IToolCallExtractor
    {
        public IEnumerable<AgentFunctionCall> Extract(
            ChatMessageContent chatMessage)
        {
            var geminiChatMessage = chatMessage as GeminiChatMessageContent ??
                throw new Exception("Este mensaje no es un mensaje de Gemini");

            return (geminiChatMessage.ToolCalls ?? [])
                .Select(
                    t => new AgentFunctionCall
                    {
                        PluginName = t.PluginName,
                        FunctionName = t.FunctionName,
                        Arguments = t.Arguments
                    });
        }
    }
}
