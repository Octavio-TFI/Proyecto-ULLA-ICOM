using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Abstractions.DTOs
{
    public record CalificacionMensajeDTO
    {
        /// <summary>
        /// Platforma utilizada para enviar el mensaje
        /// </summary>
        public required string Plataforma { get; init; }

        /// <summary>
        /// Identificador del mensaje en la plataforma
        /// </summary>
        public required string IdMensajePlataforma { get; init; }

        /// <summary>
        /// Calificación del mensaje
        /// </summary>
        public required bool Calificacion { get; init; }
    }
}
