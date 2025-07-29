using Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado
{
    public class MensajeHerramienta
        : Mensaje
        , IMensajeTexto
    {
        // Este texto no es necesario con la lista de documentos generados en suficiente
        public required string Texto { get; init; }

        public List<DocumentoRecuperado> DocumentosRecuperados { get; } = [];

        public List<ConsultaRecuperada> ConsultasRecuperadas { get; } = [];

        public override string ToString()
        {
            return Texto;
        }
    }
}
