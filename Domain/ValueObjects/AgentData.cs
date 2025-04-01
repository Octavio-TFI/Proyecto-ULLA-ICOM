using Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public record AgentData
    {
        /// <summary>
        /// Información recuperada por el agente que se utlizo para generar la respuesta
        /// </summary>
        public List<IInformacionRecuperada> InformacionRecuperada { get; } = [];

        /// <summary>
        /// Metadatos del agente
        /// </summary>
        public Dictionary<string, object> MetaData { get; } = [];
    }
}
