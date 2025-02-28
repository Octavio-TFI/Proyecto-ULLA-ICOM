using AppServices.Ports;
using Domain;
using Domain.Abstractions;
using Domain.Entities;
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
    internal class MensajeRecibidoHandler(
        IChatRepository chatRepository,
        [FromKeyedServices(TipoAgent.Chat)] IAgent agent,
        [FromKeyedServices(Contexts.Chat)] IUnitOfWork unitOfWork,
        ILogger<MensajeGeneradoHandler> logger)
        : INotificationHandler<MensajeRecibidoEvent>
    {
        readonly IChatRepository _chatRepository = chatRepository;
        readonly IAgent _agent = agent;
        readonly IUnitOfWork _unitOfWork = unitOfWork;
        readonly ILogger<MensajeGeneradoHandler> _logger = logger;

        public async Task Handle(
            MensajeRecibidoEvent notification,
            CancellationToken cancellationToken)
        {
            var chat = await _chatRepository
                .GetWithUltimosMensajesAsync(notification.EntityId)
                .ConfigureAwait(false);

            var respuesta = await chat
                .GenerarMensajeAsync(_agent)
                .ConfigureAwait(false);

            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            _logger.LogInformation(
                @"
MENSAJE GENERADO
Texto: {Texto}
ChatId: {ChatId}",
                respuesta.ToString(),
                notification.EntityId);
        }
    }
}
