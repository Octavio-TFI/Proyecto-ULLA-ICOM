using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Document
        : Entity
    {
        public required string Filename { get; set; }

        public required string Texto { get; set; }

        public ICollection<DocumentChunk> Chunks { get; set; } = [];

        public override string ToString()
        {
            return Texto;
        }
    }
}
