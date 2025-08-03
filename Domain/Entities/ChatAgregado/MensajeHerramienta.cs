using Domain.Abstractions;
using Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado
{
    public abstract class MensajeHerramienta
        : Mensaje
    {
        public required MensajeLlamadaHerramienta Llamada { get; init; }
    }
}
