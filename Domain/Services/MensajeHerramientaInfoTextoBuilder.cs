using Domain.Abstractions;
using Domain.Entities.ChatAgregado;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    internal class MensajeHerramientaInfoTextoBuilder(
        IDocumentRepository documentRepository,
        IConsultaRepository consultaRepository,
        ILogger<MensajeHerramientaInfoTextoBuilder> logger)
        : IMensajeHerramientaTextoBuilder
    {
        public async Task<string> BuildAsync(
            MensajeHerramienta mensajeHerramienta)
        {
            if (mensajeHerramienta is not MensajeHerramientaInfo mensajeHerramientaInfo)
            {
                throw new NotSupportedException(
                    $"Tipo de mensaje no soportado: {mensajeHerramienta.GetType().Name}");
            }

            // Se podria optimizar obteniendo los textos en una sola consulta
            var rankedDocuments = new List<string>();

            foreach (var doc in mensajeHerramientaInfo.DocumentosRecuperados
                .Where(d => d.Rank))
            {
                rankedDocuments.Add(
                    await documentRepository.GetTextoAsync(doc.DocumentoId));
            }

            var rankedConsultas = new List<string>();

            foreach (var consulta in mensajeHerramientaInfo.ConsultasRecuperadas
                .Where(c => c.Rank))
            {
                rankedConsultas.Add(
                    await consultaRepository.GetTextoAsync(consulta.ConsultaId));
            }


            var stringBuilder = new StringBuilder();

            stringBuilder.Append("[Documentación]").AppendLine();

            if (rankedDocuments.Count > 0)
            {
                stringBuilder
                    .AppendJoin("\r\n", rankedDocuments);
            }
            else
            {
                stringBuilder.AppendLine(
                    "No se encontro documentación relacionada");
            }

            stringBuilder.AppendLine();
            stringBuilder.Append("[Consultas Históricas]").AppendLine();

            if (rankedConsultas.Count > 0)
            {
                stringBuilder
                    .AppendJoin(
                        "\r\n",
                        rankedConsultas.Select(c => c.ToString()));
            }
            else
            {
                stringBuilder.AppendLine(
                    "No se encontraron consultas históricas similares");
            }

            string info = stringBuilder.ToString();

            logger.LogInformation(
                @"
EJECUCCION HERRAMIENTA: {Herramienta}
MENSAJE HERRAMIENTA:

{info}",
                mensajeHerramienta.Llamada.ToString(),
                info);

            return info;
        }
    }
}
