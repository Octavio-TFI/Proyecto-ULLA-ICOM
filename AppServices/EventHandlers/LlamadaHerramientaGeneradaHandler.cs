using AppServices.Ports;
using Domain.Abstractions;
using Domain.Events;
using Domain.Repositories;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.EventHandlers
{
    internal class LlamadaHerramientaGeneradaHandler(
        IChatRepository chatRepository,
        [FromKeyedServices(TipoAgent.Chat)] IAgent agent,
        ILogger<LlamadaHerramientaGeneradaHandler> logger)
        : INotificationHandler<LlamadaHerramientaGeneradaEvent>
    {
        readonly IChatRepository _chatRepository = chatRepository;
        readonly IAgent _agent = agent;
        readonly ILogger _logger = logger;

        public async Task Handle(
            LlamadaHerramientaGeneradaEvent notification,
            CancellationToken cancellationToken)
        {
            var chat = await _chatRepository
                .GetWithUltimosMensajesAsync(notification.EntityId)
                .ConfigureAwait(false);

            var mensajeHerramienta = await chat
                .LlamarHerramientaAsyn(_agent)
                .ConfigureAwait(false);

            _logger.LogInformation(
                @"
Herramienta Llamada
Herramienta: {Herramienta}
Resultado: {Resultado}
ChatId: {ChatId}",
                chat.UltimoMensaje.ToString(),
                mensajeHerramienta.ToString(),
                notification.EntityId);
        }
    }
}
