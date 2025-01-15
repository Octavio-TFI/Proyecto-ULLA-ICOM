using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Document : Entity
    {
        public required string Texto { get; set; }

        public required float[] Embedding { get; set; }

        public IEnumerable<Document> Childs { get; set; } = [];

        public override string ToString()
        {
            return base.ToString()!;
        }
    }
}
