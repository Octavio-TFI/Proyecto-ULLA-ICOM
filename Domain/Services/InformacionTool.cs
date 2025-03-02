using AppServices.Abstractions;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Entities.ChatAgregado;
using Domain.Repositories;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    internal class InformacionTool(
        ILogger<InformacionTool> _logger,
        IEmbeddingService _embeddingService,
        IConsultaRepository _consultaRepository,
        IDocumentRepository _documentRepository,
        IRanker _ranker,
        AgentData _agentData)
    {
        [DisplayName("informacion")]
        [Description(
            "Busca documentación relacionada a la pregunta del usuario y soluciones a preguntas similares")]
        public async Task<string> BuscarInformacionAsync(
            [Description("Pregunta o problema que tiene el usuario")] string pregunta)
        {
            _logger.LogInformation(
                @"
BUSCANDO INFORMACION PARA QUERY:
{Query}
",
                pregunta);

            var embeddingConsulta = await _embeddingService
                .GenerateAsync(pregunta)
                .ConfigureAwait(false);

            var consultas = await _consultaRepository
                .GetConsultasSimilaresAsync(embeddingConsulta)
                .ConfigureAwait(false);

            var rankedConsultas = await _ranker.RankAsync(consultas, pregunta)
                .ConfigureAwait(false);

            _logger.LogInformation(
                @"
SE ENCONTRARON {consultasCount} CONSULTAS PARA QUERY:
{Query}
",
                rankedConsultas.Count,
                pregunta);

            var documents = await _documentRepository
                .GetDocumentosRelacionadosAsync(embeddingConsulta)
                .ConfigureAwait(false);

            var rankedDocuments = await _ranker.RankAsync(documents, pregunta)
                .ConfigureAwait(false);

            _logger.LogInformation(
                @"
SE ENCONTRARON {rankedDocuments} DOCUMENTOS PARA QUERY:
{Query}
",
                rankedDocuments.Count,
                pregunta);

            var stringBuilder = new StringBuilder();

            stringBuilder.Append("[Documentación]").AppendLine();

            if (rankedDocuments.Count > 0)
            {
                stringBuilder
                    .AppendJoin(
                        "\r\n",
                        rankedDocuments.Select(d => d.ToString()));
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

            _logger.LogInformation(
                @"
INFORMACION PARA QUERY: {query}

{info}
",
                pregunta,
                info);

            // Guardar metadata de los datos recuperados
            _agentData.InformacionRecuperada
                .AddRange(
                    consultas.Select(
                        c => new ConsultaRecuperada
                        {
                            ConsultaId = c.Id,
                            Rank = rankedConsultas.Contains(c)
                        }));

            _agentData.InformacionRecuperada
                .AddRange(
                    documents.Select(
                        d => new DocumentoRecuperado
                        {
                            DocumentoId = d.Id,
                            Rank = rankedDocuments.Contains(d)
                        }));


            return info;
        }
    }
}
