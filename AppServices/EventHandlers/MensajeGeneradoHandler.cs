using AppServices.Ports;
using Domain.Events;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.EventHandlers
{
    internal class MensajeGeneradoHandler(
        IChatRepository _chatRepository,
        Func<string, IClient> _clientFactory)
        : INotificationHandler<MensajeGeneradoEvent>
    {
        public async Task Handle(
            MensajeGeneradoEvent mensajeGeneradoEvent,
            CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetAsync(
                mensajeGeneradoEvent.Mensaje.ChatId);

            var client = _clientFactory.Invoke(chat.Plataforma);

            await client.EnviarMensaje(
                chat.ChatPlataformaId,
                chat.UsuarioId,
                mensajeGeneradoEvent.Mensaje);
        }
    }
}
