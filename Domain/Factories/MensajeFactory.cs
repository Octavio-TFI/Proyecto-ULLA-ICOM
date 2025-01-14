using Domain.Abstractions.Factories;
using Domain.Entities;
using Domain.Events;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Factories
{
    internal class MensajeFactory
        : IMensajeFactory
    {
        public MensajeTexto CreateMensajeTextoRecibido(
            int chatId,
            DateTime dateTime,
            TipoMensaje tipoMensaje,
            string texto)
        {
            var mensaje = new MensajeTexto
            {
                ChatId = chatId,
                DateTime = dateTime,
                Tipo = tipoMensaje,
                Texto = texto
            };

            mensaje.Events.Add(new MensajeRecibidoEvent { ChatId = chatId });

            return mensaje;
        }

        public MensajeTexto CreateMensajeTextoGenerado(int chatId, string texto)
        {
            var mensaje = new MensajeTexto
            {
                ChatId = chatId,
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Asistente,
                Texto = texto
            };

            mensaje.Events.Add(new MensajeGeneradoEvent { ChatId = chatId });

            return mensaje;
        }
    }
}
