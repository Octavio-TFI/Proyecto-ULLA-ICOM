using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions.Factories
{
    public interface IMensajeFactory
    {
        /// <summary>
        /// Crea un mensaje de texto
        /// </summary>
        /// <param name="chatId">Id del chat</param>
        /// <param name="dateTime">DateTime de cuando se recibio el mensaje</param>
        /// <param name="tipoMensaje">
        /// Tipo del mensaje.
        /// Enviado por usuario o generado por el asistente(LLM)</param>
        /// <param name="texto">Texto del mensaje</param>
        /// <returns></returns>
        MensajeTexto CreateMensajeTexto(
            int chatId,
            DateTime dateTime,
            TipoMensaje tipoMensaje,
            string texto);
    }
}
