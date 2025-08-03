using Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public record ToolResult
    {
        /// <summary>
        /// Datos sobre la ejecucion de la herramienta
        /// </summary>
        public required ReadOnlyDictionary<string, object> Datos { get; init; }
    }
}
