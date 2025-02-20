using AppServices;
using AppServices.Abstractions;
using AppServices.Agents;
using AppServices.Ports;
using Domain.Abstractions.Factories;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    internal class GeneradorRespuesta(
        [FromKeyedServices(TipoAgent.Chat)] ChatCompletionAgent agent,
        IMensajeFactory mensajeFactory,
        IChatHistoryFactory chatHistoryFactory)
        : IGeneradorRespuesta
    {
        readonly ChatCompletionAgent _agent = agent;
        readonly IMensajeFactory _mensajeFactory = mensajeFactory;
        readonly IChatHistoryFactory _chatHistoryFactory = chatHistoryFactory;

        public async Task<Mensaje> GenerarRespuestaAsync(List<Mensaje> mensajes)
        {
            var chatHistory = _chatHistoryFactory.Create(mensajes);

            var result = await _agent.InvokeAsync(chatHistory).FirstAsync();

            return _mensajeFactory.CreateMensajeTextoGenerado(
                chatId: mensajes.First().ChatId,
                texto: result.ToString());
        }
    }
}
