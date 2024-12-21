using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public record MensajeRecibidoEvent<M> : INotification
        where M : Mensaje
    {
        public required M Mensaje { get; init; }
    }
}
