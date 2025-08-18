using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public record MensajeHerramientaGeneradoEvent
        : EntityEvent
    {
        /// <summary>
        /// Id del mensaje generado por la herramienta
        /// </summary>
        public required Guid MensajeHerramientaId { get; init; }

        public override int MaxRetries => 5;

        public override TimeSpan RetryInterval => TimeSpan.FromSeconds(5);
    }
}
