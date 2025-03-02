using Domain.Entities.ChatAgregado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Ports
{
    /// <summary>
    /// Interfaz que representa un cliente de una plataforma de mensajeria
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Envia un mensaje a un chat de una plataforma
        /// </summary>
        /// <param name="chatPlataformaId">Chat al cual enviar el mensaje</param>
        /// <param name="usuarioId">Usuario al cual enviar el mensaje</param>
        /// <param name="mensaje">Mensaje a enviar</param>
        Task EnviarMensajeAsync(
            string chatPlataformaId,
            string usuarioId,
            Mensaje mensaje);
    }
}
