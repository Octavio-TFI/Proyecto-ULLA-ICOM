using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; init; } = Guid.NewGuid();

        /// <summary>
        /// Lista de eventos
        /// </summary>
        public List<INotification> Events { get; } = [];
    }
}
