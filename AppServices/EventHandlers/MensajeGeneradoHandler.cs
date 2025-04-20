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
        IMensajeIARepository _mensajeIARepository,
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

            var mensaje = await _mensajeIARepository.GetAsync(
                mensajeGeneradoEvent.MensajeId);

            var client = _clientFactory.Invoke(chat.Plataforma);

            string mensajePlataformaId = await client.EnviarMensajeAsync(
                chat.ChatPlataformaId,
                chat.UsuarioId,
                mensaje)
                .ConfigureAwait(false);

            _logger.LogInformation(
                @"
MENSAJE ENVIADO
Texto: {Texto}
ChatId: {ChatId}",
                mensaje.ToString(),
                mensajeGeneradoEvent.EntityId);

            mensaje.PlataformaMensajeId = mensajePlataformaId;
        }
    }
}
