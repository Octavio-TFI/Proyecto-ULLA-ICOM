using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public record LlamadaHerramientaGeneradaEvent
        : EntityEvent
    {
        /// <summary>
        /// Id del mensaje de llamada de herramienta generado.
        /// </summary>
        public required Guid MensajeLlamadaHerramientaGeneradaId { get; init; }

        public override int MaxRetries => 5;

        public override TimeSpan RetryInterval => TimeSpan.FromSeconds(5);
    }
}
