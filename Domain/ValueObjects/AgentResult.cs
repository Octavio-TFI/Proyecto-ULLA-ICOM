using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public record AgentResult
    {
        /// <summary>
        /// Texto generado por el agente
        /// </summary>
        public required string Texto { get; init; }

        /// <summary>
        /// Datos del agente
        /// </summary>
        public required AgentData AgentData { get; init; }
    }
}
