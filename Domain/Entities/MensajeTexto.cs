using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MensajeTexto : Mensaje
    {
        /// <summary>
        /// Texto del mensaje
        /// </summary>
        public required string Texto { get; init; }
    }
}
