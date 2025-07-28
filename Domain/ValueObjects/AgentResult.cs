using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public abstract record AgentResult
    {
        /// <summary>
        /// Datos del agente
        /// </summary>
        public required AgentData AgentData { get; init; }
    }
}
