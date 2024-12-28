using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Ports
{
    internal interface IGeneradorRespuesta
    {
        /// <summary>
        /// Genera una respuesta a partir de ultimos mensajes de un chat.
        /// </summary>
        /// <param name="mensajes">Ultimos mensajes de un chat</param>
        /// <returns>Respuesta generada por el LLM</returns>
        Task<Mensaje> GenerarRespuestaAsync(List<Mensaje> mensajes);
    }
}
