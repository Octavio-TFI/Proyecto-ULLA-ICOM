using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public record AgentTextResult
        : AgentResult
    {
        /// <summary>
        /// Texto generado por el agente.
        /// </summary>
        public required string Texto { get; init; }

        public override string ToString()
        {
            return Texto;
        }
    }
}
