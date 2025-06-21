using AppServices.Abstractions.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Abstractions
{
    /// <summary>
    /// Interfaz para calificar mensajes generados por la IA
    /// </summary>
    public interface ICalificadorMensajes
    {
        /// <summary>
        /// Califica un mensaje generado por la IA
        /// </summary>
        /// <param name="calificacionMensaje">Calificaion del mensaje recibida</param>
        Task CalificarMensajeAsync(CalificacionMensajeDTO calificacionMensaje);
    }
}
