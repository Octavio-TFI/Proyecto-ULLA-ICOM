using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.DocumentoAgregado
{
    public class DocumentChunk
        : Entity
    {
        public required string Texto { get; set; }

        public required float[] Embedding { get; set; }
    }
}
