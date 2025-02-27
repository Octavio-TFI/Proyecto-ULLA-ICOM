using Domain.Entities;
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
        /// <returns>Respuesta generada por el agente</returns>
        Task<string> GenerarRespuestaAsync(List<Mensaje> mensajes);

        /// <summary>
        /// Genera una respuesta a partir de un mensaje.
        /// </summary>
        /// <param name="mensaje">Mensaje</param>
        /// <returns>Respuesta generada por el agente</returns>
        Task<string> GenerarRespuestaAsync(string mensaje);
    }
}
