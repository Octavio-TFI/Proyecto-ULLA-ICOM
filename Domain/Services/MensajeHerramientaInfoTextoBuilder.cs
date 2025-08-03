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

            var rankedDocuments = await Task.WhenAll(
                mensajeHerramientaInfo.DocumentosRecuperados
                    .Where(d => d.Rank)
                    .Select(
                        async d => await documentRepository.GetTextoAsync(
                                d.DocumentoId)));

            var rankedConsultas = await Task.WhenAll(
                mensajeHerramientaInfo.ConsultasRecuperadas
                    .Where(c => c.Rank)
                    .Select(
                        async c => await consultaRepository.GetTextoAsync(
                                c.ConsultaId)));


            var stringBuilder = new StringBuilder();

            stringBuilder.Append("[Documentación]").AppendLine();

            if (rankedDocuments.Length > 0)
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

            if (rankedConsultas.Length > 0)
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
