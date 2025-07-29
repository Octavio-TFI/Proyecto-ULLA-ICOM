using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado
{
    public class MensajeLlamadaHerramienta
        : Mensaje
    {
        /// <summary>
        /// Nombre del plugin que se invoca
        /// </summary>
        public required string? PluginName { get; init; }

        /// <summary>
        /// Nombre de la función que se invoca
        /// </summary>
        public required string FunctionName { get; init; }

        /// <summary>
        /// Argumentos que se pasan a la función
        /// </summary>
        public required IReadOnlyDictionary<string, object?>? Argumentos { get; init; }

        public override string ToString()
        {
            var plugin = PluginName ?? "N/A";

            var args = Argumentos != null
                ? JsonConvert.SerializeObject(Argumentos)
                : "N/A";

            return $"Plugin: {plugin}, Function: {FunctionName}, Args: {args}";
        }
    }
}
