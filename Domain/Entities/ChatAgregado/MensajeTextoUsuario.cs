using Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado
{
    public class MensajeTextoUsuario
        : Mensaje
        , IMensajeTexto
    {
        internal MensajeTextoUsuario()
        {
        }

        public required string Texto { get; init; }

        public override string ToString()
        {
            return Texto;
        }
    }
}
