using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class EmbeddingDoc : Entity
    {
        public required string Titulo { get; set; }

        public required string Texto { get; set; }

        public required string EmbeddingTitulo { get; set; }

        public required string EmbeddingTexto { get; set; }
    }
}
