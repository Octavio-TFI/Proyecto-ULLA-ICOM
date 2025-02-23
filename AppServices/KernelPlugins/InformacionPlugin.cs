﻿using AppServices.Abstractions;
using Domain.Entities;
using Domain.Repositories;
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
        Kernel _kernel,
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
            var embeddingConsulta = await _kernel
                .GetRequiredService<ITextEmbeddingGenerationService>()
                .GenerateEmbeddingAsync(pregunta);

            var consultas = await _consultaRepository
                .GetConsultasSimilaresAsync(embeddingConsulta);

            var rankedConsultas = await _ranker.RankAsync(consultas, pregunta);

            var documents = await _documentRepository
                .GetDocumentosRelacionadosAsync(embeddingConsulta);

            var rankedDocuments = await _ranker.RankAsync(documents, pregunta);

            var stringBuilder = new StringBuilder();

            if (rankedDocuments.Count > 0)
            {
                stringBuilder.Append("[Documentación]")
                    .AppendLine()
                    .AppendJoin(
                        "\r\n",
                        rankedDocuments.Select(d => d.ToString()));
            }

            if (rankedConsultas.Count > 0)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.AppendLine();
                }

                stringBuilder.Append("[Consultas Históricas]")
                    .AppendLine()
                    .AppendJoin(
                        "\r\n",
                        rankedConsultas.Select(c => c.ToString()));
            }

            return stringBuilder.ToString();
        }
    }
}
