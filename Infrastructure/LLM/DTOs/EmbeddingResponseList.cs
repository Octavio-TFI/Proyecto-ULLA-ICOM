using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.DTOs
{
    internal class EmbeddingResponseList
    {
        public required List<EmbeddingResponse> Data { get; init; }
    }
}
