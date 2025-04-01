using Domain.Entities.ChatAgregado;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Domain.Events
{
    public record MensajeGeneradoEvent
        : EntityEvent
    {
        public required Mensaje Mensaje { get; init; }
    }
}
