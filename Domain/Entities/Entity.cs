﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public abstract class Entity
    {
        /// <summary>
        /// Id del mensaje
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Lista de eventos
        /// </summary>
        public List<INotification> Events { get; } = [];
    }
}
