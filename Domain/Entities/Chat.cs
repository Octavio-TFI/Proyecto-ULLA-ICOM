using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    internal class Chat
    {
        public int Id { get; set; }

        public required string UsuarioId { get; init; }

        public required string ChatId { get; init; }

        public required string Plataforma { get; init; }
    }
}
