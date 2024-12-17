using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    internal abstract class Mensaje
    {
        /// <summary>
        /// Id del mensaje
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// DateTime de cuando se recibio el mensaje
        /// </summary>
        public DateTime DateTime { get; init; }

        /// <summary>
        /// Id del Chat
        /// </summary>
        public int ChatId { get; init; }
    }
}
