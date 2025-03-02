using Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado
{
    public class MensajeIA : Mensaje, IMensajeTexto
    {
        internal MensajeIA()
        {
        }

        public string Texto { get; init; }

        public bool? Calificacion { get; set; }

        public override string ToString()
        {
            return Texto;
        }
    }
}
