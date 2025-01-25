using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Clients
{
    internal record TestDTO
    {
        public required string ChatId { get; init; }

        public required string Texto { get; init; }
    }
}
