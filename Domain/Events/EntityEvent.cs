using Domain.Entities;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public abstract record EntityEvent
        : INotification
    {
        public required Guid EntityId { get; init; }

        /// <summary>
        /// Maxima cantidad de reintentos para procesar el evento.
        /// </summary>
        [JsonIgnore]
        public abstract int MaxRetries { get; }

        /// <summary>
        /// Intervalo entre reintentos para este tipo de evento.
        /// </summary>
        [JsonIgnore]
        public abstract TimeSpan RetryInterval { get; }
    }
}
