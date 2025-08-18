using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado
{
    public class MensajeHerramientaInfo
        : MensajeHerramienta
    {
        protected MensajeHerramientaInfo()
        {
            _documentosRecuperados = [];
            _consultasRecuperadas = [];
        }

        public MensajeHerramientaInfo(
            List<DocumentoRecuperado> documentosRecuperados,
            List<ConsultaRecuperada> consultasRecuperadas)
        {
            _documentosRecuperados = documentosRecuperados;
            _consultasRecuperadas = consultasRecuperadas;
        }

        readonly List<DocumentoRecuperado> _documentosRecuperados;
        readonly List<ConsultaRecuperada> _consultasRecuperadas;

        public virtual IReadOnlyList<DocumentoRecuperado> DocumentosRecuperados
            => _documentosRecuperados;

        public virtual IReadOnlyList<ConsultaRecuperada> ConsultasRecuperadas
            => _consultasRecuperadas;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Documentos Recuperados: {DocumentosRecuperados.Count}");

            if (DocumentosRecuperados.Count > 0)
            {
                sb.Append('[');
                sb.Append(
                    string.Join(
                        ", ",
                        DocumentosRecuperados.Select(d => d.DocumentoId)));
                sb.Append(']');
            }

            sb.Append($", Consultas Recuperadas: {ConsultasRecuperadas.Count}");

            if (ConsultasRecuperadas.Count > 0)
            {
                sb.Append('[');
                sb.Append(
                    string.Join(
                        ", ",
                        ConsultasRecuperadas.Select(c => c.ConsultaId)));
                sb.Append(']');
            }

            return sb.ToString();
        }
    }
}
