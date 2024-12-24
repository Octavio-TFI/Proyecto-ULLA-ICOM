using Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.EventHandlers
{
    internal class MensajeRecibidoHandler : INotificationHandler<MensajeRecibidoEvent>
    {
        public Task Handle(
            MensajeRecibidoEvent request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
