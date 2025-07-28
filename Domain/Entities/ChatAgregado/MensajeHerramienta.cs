using Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado
{
    public class MensajeHerramienta
        : Mensaje
        , IMensajeTexto
    {
        public required string Texto { get; init; }

        public override string ToString()
        {
            return Texto;
        }
    }
}
