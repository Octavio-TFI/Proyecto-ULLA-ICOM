﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado
{
    public class DocumentoRecuperado : Entity
    {
        internal DocumentoRecuperado()
        {
        }

        /// <summary>
        /// Id del documento recuperado
        /// </summary>
        public required Guid DocumentoId { get; init; }

        /// <summary>
        /// Indica si el documento recuperado fue relevante y se utilizo para responder
        /// </summary>
        public required bool Rank { get; init; }
    }
}
