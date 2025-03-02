using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ConsultaAgregado
{
    public class Consulta
        : Entity
    {
        internal Consulta()
        {
        }

        /// <summary>
        /// Id de la consulta en la base de datos original
        /// </summary>
        public required int RemoteId { get; set; }

        public required string Titulo { get; set; }

        public required string Descripcion { get; set; }

        public required string Solucion { get; set; }

        public required float[] EmbeddingTitulo { get; set; }

        public required float[] EmbeddingDescripcion { get; set; }

        public override string ToString()
        {
            return base.ToString()!;
        }
    }
}
