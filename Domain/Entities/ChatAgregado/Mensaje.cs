using Domain.Events;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado
{
    public abstract class Mensaje : Entity
    {
        /// <summary>
        /// DateTime de cuando se recibio el mensaje
        /// </summary>
        public required DateTime DateTime { get; set; }
    }
}
