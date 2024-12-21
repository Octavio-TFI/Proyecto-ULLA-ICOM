using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public abstract class Mensaje
    {
        /// <summary>
        /// Id del mensaje
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// DateTime de cuando se recibio el mensaje
        /// </summary>
        public required DateTime DateTime { get; init; }

        /// <summary>
        /// Id del Chat
        /// </summary>
        public required int ChatId { get; init; }
    }
}
