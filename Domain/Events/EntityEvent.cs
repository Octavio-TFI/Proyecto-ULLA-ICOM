using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public abstract record EntityEvent : INotification
    {
        public required int EntityId { get; init; }
    }
}
