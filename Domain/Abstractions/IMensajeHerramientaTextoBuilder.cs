using Domain.Entities.ChatAgregado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IMensajeHerramientaTextoBuilder
    {
        /// <summary>
        /// Construye el texto de un mensaje de herramienta.
        /// </summary>
        /// <param name="mensajeHerramienta">El mensaje de la herramienta</param>
        /// <returns>El texto construido del mensaje de la herramienta.</returns>
        Task<string> BuildAsync(MensajeHerramienta mensajeHerramienta);
    }
}
