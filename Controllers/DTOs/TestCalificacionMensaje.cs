using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers.DTOs
{
    public record TestCalificacionMensaje
    {
        public required string MensajeId { get; init; }

        public required bool Calificacion { get; init; }
    }
}
