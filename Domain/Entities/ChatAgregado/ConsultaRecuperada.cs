using Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado
{
    public class ConsultaRecuperada : Entity, IInformacionRecuperada
    {
        internal ConsultaRecuperada()
        {
        }

        public Guid InformacionId => ConsultaId;

        /// <summary>
        /// Id de la consulta recuperada
        /// </summary>
        public required Guid ConsultaId { get; init; }

        /// <summary>
        /// Indica si la consulta recuperada fue relevante y se utilizo para responder
        /// </summary>
        public required bool Rank { get; init; }
    }
}
