using Domain.Entities;
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
        /// Envia un mensaje a un destinatario
        /// </summary>
        /// <param name="mensaje">Mensaje a enviar</param>
        Task EnviarMensaje(Mensaje mensaje);
    }
}
