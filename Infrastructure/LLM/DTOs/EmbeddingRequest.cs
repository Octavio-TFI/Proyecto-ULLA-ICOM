using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.DTOs
{
    internal record EmbeddingRequest
    {
        public required IList<string> Input { get; init; }

        public required string Model { get; init; }
    }
}
