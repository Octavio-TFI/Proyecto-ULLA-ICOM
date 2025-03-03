using Domain.Abstractions;
using Domain.Abstractions.Factories;
using Domain.Entities.ChatAgregado;
using Domain.ValueObjects;
using Infrastructure.LLM.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM
{
    internal class Agent(
        ChatCompletionAgent agent,
        AgentData agentData,
        IChatHistoryAdapter chatHistoryFactory)
        : IAgent
    {
        public ChatCompletionAgent ChatCompletionAgent { get; } = agent;

        readonly AgentData _data = agentData;
        readonly IChatHistoryAdapter _chatHistoryFactory = chatHistoryFactory;

        public Task<AgentResult> GenerarRespuestaAsync(
            List<Mensaje> mensajes,
            Dictionary<string, object?>? arguments = null)
        {
            var chatHistory = _chatHistoryFactory.Adapt(mensajes);

            return GenerarRespuestaAsync(chatHistory, arguments);
        }

        public Task<AgentResult> GenerarRespuestaAsync(
            string mensaje,
            Dictionary<string, object?>? arguments = null)
        {
            var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage(mensaje);

            return GenerarRespuestaAsync(chatHistory, arguments);
        }

        async Task<AgentResult> GenerarRespuestaAsync(
            ChatHistory chatHistory,
            Dictionary<string, object?>? arguments = null)
        {
            var kernelArguments = new KernelArguments(arguments ?? []);

            var result = await ChatCompletionAgent
                .InvokeAsync(chatHistory, kernelArguments)
                .FirstAsync()
                .ConfigureAwait(false);

            return new AgentResult
            {
                Texto = result.ToString(),
                AgentData = _data,
            };
        }
    }
}
