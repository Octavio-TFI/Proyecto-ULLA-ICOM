using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions.Entities
{
    public interface IInformacionRecuperada
    {
        /// <summary>
        /// Id de la información recuperada
        /// </summary>
        public Guid InformacionId { get; }

        /// <summary>
        /// Indica si la informacion recuperada fue relevante y se utilizo para responder
        /// </summary>
        public bool Rank { get; }
    }
}
