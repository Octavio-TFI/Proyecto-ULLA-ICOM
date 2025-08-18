using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public record MensajeRecibidoEvent
        : EntityEvent
    {
        public override int MaxRetries => 5;

        public override TimeSpan RetryInterval => TimeSpan.FromSeconds(5);
    }
}
