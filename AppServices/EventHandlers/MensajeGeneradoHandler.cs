using AppServices.Ports;
using Domain.Events;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.EventHandlers
{
    internal class MensajeGeneradoHandler(
        IChatRepository _chatRepository,
        Func<string, IClient> _clientFactory,
        ILogger<MensajeGeneradoHandler> _logger)
        : INotificationHandler<MensajeGeneradoEvent>
    {
        public async Task Handle(
            MensajeGeneradoEvent mensajeGeneradoEvent,
            CancellationToken cancellationToken)
        {
            var chat = await _chatRepository
                .GetAsync(mensajeGeneradoEvent.EntityId)
                .ConfigureAwait(false);

            var client = _clientFactory.Invoke(chat.Plataforma);

            await client.EnviarMensajeAsync(
                chat.ChatPlataformaId,
                chat.UsuarioId,
                mensajeGeneradoEvent.Mensaje)
                .ConfigureAwait(false);

            _logger.LogInformation(
                @"
MENSAJE ENVIADO
Texto: {Texto}
ChatId: {ChatId}",
                mensajeGeneradoEvent.Mensaje.ToString(),
                mensajeGeneradoEvent.Mensaje.ChatId);
        }
    }
}
