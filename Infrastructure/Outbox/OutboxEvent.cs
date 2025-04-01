using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Outbox
{
    internal class OutboxEvent
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Tipo del evento ocurrido
        /// </summary>
        public required string EventType { get; set; }

        /// <summary>
        /// Datos del evento ocurrido (JSON del objeto evento)
        /// </summary>
        public required string EventData { get; set; }

        /// <summary>
        /// Fecha y hora en la que ocurrió el evento
        /// </summary>
        public required DateTime OccurredOn { get; set; }

        /// <summary>
        /// Fecha y hora en la que se procesó el evento
        /// </summary>
        public DateTime? ProcessedOn { get; set; }

        /// <summary>
        /// Indica si el evento ya fue procesado
        /// </summary>
        public bool IsProcessed { get; set; }
    }
}
