using Domain.Abstractions;
using Domain.Events;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Chat : Entity
    {
        /// <summary>
        /// Id del usuario en la plataforma que envia el mensaje
        /// </summary>
        public required string UsuarioId { get; init; }

        /// <summary>
        /// Id del chat en la plataforma que envia el mensaje
        /// </summary>
        public required string ChatPlataformaId { get; init; }

        /// <summary>
        /// Platforma utilizada para enviar el mensaje
        /// </summary>
        public required string Plataforma { get; init; }

        /// <summary>
        /// Lista de mensajes
        /// </summary>
        public List<Mensaje> Mensajes { get; } = [];

        /// <summary>
        /// Añade un mensaje de texto recibido
        /// </summary>
        /// <param name="dateTime">Date Time de cuando se envio el mensaje</param>
        /// <param name="texto">Texto del mensaje</param>
        public virtual Mensaje AñadirMensajeTextoRecibido(
            DateTime dateTime,
            string texto)
        {
            var mensaje = new MensajeTexto
            {
                DateTime = dateTime,
                Tipo = TipoMensaje.Usuario,
                Texto = texto
            };

            Mensajes.Add(mensaje);
            Events.Add(new MensajeRecibidoEvent { EntityId = Id });

            return mensaje;
        }

        /// <summary>
        /// Generar un mensaje de respuesta y lo añade a la lista de mensajes
        /// </summary>
        /// <param name="agente">Agente</param>
        /// <returns>Mensaje de respuesta generado</returns>
        public virtual async Task<Mensaje> GenerarMensajeAsync(
            IAgent agente)
        {
            var stringRespuesta = await agente
                .GenerarRespuestaAsync(Mensajes)
                .ConfigureAwait(false);

            var respuesta = new MensajeTexto
            {
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Asistente,
                Texto = stringRespuesta
            };

            Mensajes.Add(respuesta);
            Events.Add(
                new MensajeGeneradoEvent { EntityId = Id, Mensaje = respuesta });

            return respuesta;
        }
    }
}
