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
    public record MensajeIAGeneradoEvent
        : EntityEvent
    {
        /// <summary>
        /// Id del mensaje generado.
        /// </summary>
        public required Guid MensajeId { get; init; }

        public override int MaxRetries => 5;

        public override TimeSpan RetryInterval => TimeSpan.FromSeconds(5);
    }
}
