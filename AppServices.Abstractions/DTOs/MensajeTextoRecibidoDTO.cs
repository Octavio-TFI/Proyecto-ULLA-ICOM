﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Abstractions.DTOs
{
    public record MensajeTextoRecibidoDTO
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
        /// Texto del mensaje
        /// </summary>
        public required string Texto { get; init; }

        /// <summary>
        /// Platforma utilizada para enviar el mensaje
        /// </summary>
        public required string Plataforma { get; init; }

        /// <summary>
        /// DateTime de cuando se recibio el mensaje
        /// </summary>
        public required DateTime DateTime { get; init; }
    }
}
