using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions.Entities
{
    public interface IMensajeTexto
    {
        /// <summary>
        /// Texto del mensaje
        /// </summary>
        public string Texto { get; init; }
    }
}
