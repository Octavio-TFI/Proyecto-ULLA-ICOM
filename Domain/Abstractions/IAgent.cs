using Domain.Entities.ChatAgregado;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IAgent
    {
        /// <summary>
        /// Genera una respuesta a partir de mensajes.
        /// </summary>
        /// <param name="mensajes">Mensajes</param>
        /// <param name="arguments">Argumentos</param>
        /// <returns>Respuesta generada por el agente</returns>
        Task<AgentResult> GenerarRespuestaAsync(
            List<Mensaje> mensajes,
            Dictionary<string, object?>? arguments = null);

        /// <summary>
        /// Genera una respuesta a partir de un mensaje.
        /// </summary>
        /// <param name="mensaje">Mensaje</param>
        /// <param name="arguments">Argumentos</param>
        /// <returns>Respuesta generada por el agente</returns>
        Task<AgentResult> GenerarRespuestaAsync(
            string mensaje,
            Dictionary<string, object?>? arguments = null);
    }
}
