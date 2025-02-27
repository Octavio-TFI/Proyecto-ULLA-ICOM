using Domain.Abstractions;
using Domain.Abstractions.Factories;
using Domain.Entities;
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
        IChatHistoryFactory chatHistoryFactory)
        : IAgent
    {
        readonly ChatCompletionAgent _agent = agent;
        readonly IChatHistoryFactory _chatHistoryFactory = chatHistoryFactory;

        public async Task<string> GenerarRespuestaAsync(
            List<Mensaje> mensajes,
            Dictionary<string, object?>? arguments = null)
        {
            var chatHistory = _chatHistoryFactory.Create(mensajes);

            var kernelArguments = new KernelArguments(arguments ?? []);

            var result = await _agent.InvokeAsync(chatHistory, kernelArguments)
                .FirstAsync()
                .ConfigureAwait(false);

            return result.ToString();
        }

        public async Task<string> GenerarRespuestaAsync(
            string mensaje,
            Dictionary<string, object?>? arguments = null)
        {
            var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage(mensaje);

            var kernelArguments = new KernelArguments(arguments ?? []);

            var result = await _agent.InvokeAsync(chatHistory, kernelArguments)
                .FirstAsync()
                .ConfigureAwait(false);

            return result.ToString();
        }
    }
}
