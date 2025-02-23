﻿using Domain.Events;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public abstract class Mensaje : Entity
    {
        /// <summary>
        /// DateTime de cuando se recibio el mensaje
        /// </summary>
        public required DateTime DateTime { get; init; }

        /// <summary>
        /// Id del Chat
        /// </summary>
        public required int ChatId { get; init; }

        /// <summary>
        /// Tipo del mensaje.
        /// Enviado por usuario o generado por el asistente(LLM)
        /// </summary>
        public required TipoMensaje Tipo { get; init; }
    }
}
