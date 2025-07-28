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
        IChatHistoryAdapter chatHistoryFactory)
        : IAgent
    {
        public ChatCompletionAgent ChatCompletionAgent { get; } = agent;

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

            var functionCalls = FunctionCallContent.GetFunctionCalls(result)
                .Select(
                    f => new AgentFunctionCall
                    {
                        PluginName = f.PluginName,
                        FunctionName = f.FunctionName,
                        Arguments = f.Arguments?.ToDictionary()
                    });

            return new AgentResult
            {
                Texto = result.ToString(),
                FunctionCalls = [.. functionCalls]
            };
        }

        public async Task<ToolResult> LlamarHerramientaAsync(
            MensajeLlamadaHerramienta llamadaHerramienta)
        {
            var kernelArguments = new KernelArguments(
                llamadaHerramienta.Argumentos ?? []);

            var result = await ChatCompletionAgent.Kernel
                .InvokeAsync(
                    llamadaHerramienta.PluginName,
                    llamadaHerramienta.FunctionName,
                    kernelArguments);

            return result.GetValue<ToolResult>() ??
                throw new Exception(
                    "No se pudo obtener el resultado de la herramienta");
        }
    }
}
