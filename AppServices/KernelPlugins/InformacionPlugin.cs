using AppServices.Abstractions;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.KernelPlugins
{
    internal class InformacionPlugin(
        ILogger<InformacionPlugin> _logger,
        ITextEmbeddingGenerationService _embeddingService,
        IConsultaRepository _consultaRepository,
        IDocumentRepository _documentRepository,
        IRanker _ranker)
    {
        [KernelFunction("informacion")]
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
                .GenerateEmbeddingAsync(pregunta);

            var consultas = await _consultaRepository
                .GetConsultasSimilaresAsync(embeddingConsulta);

            var rankedConsultas = await _ranker.RankAsync(consultas, pregunta);

            _logger.LogInformation(
                @"
SE ENCONTRARON {consultasCount} CONSULTAS PARA QUERY:
{Query}
",
                rankedConsultas.Count,
                pregunta);

            var documents = await _documentRepository
                .GetDocumentosRelacionadosAsync(embeddingConsulta);

            var rankedDocuments = await _ranker.RankAsync(documents, pregunta);

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

            return info;
        }
    }
}
