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
        public Task Handle(
            MensajeGeneradoEvent notification,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
