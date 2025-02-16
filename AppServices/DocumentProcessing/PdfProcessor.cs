using AppServices.Abstractions;
using Domain.Entities;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.Extensions.DependencyInjection;
using ReverseMarkdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppServices.DocumentProcessing
{
    internal partial class PdfProcessor(
        [FromKeyedServices(".md")] IDocumentProcessor _markdownProcessor)
        : IDocumentProcessor
    {
        public async Task<Document> ProcessAsync(
            string path,
            byte[] documentData)
        {
            using MemoryStream pdfStream = new(documentData);

            using var pdfReader = new PdfReader(
                pdfStream,
                new ReaderProperties { });
            using var pdf = new PdfDocument(pdfReader);

            var pdfStringBuilder = new StringBuilder();

            List<string> posibleTitles = [];

            for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
            {
                var strategy = new PdfTextExtractionStrategy();

                var page = pdf.GetPage(i);
                var text = PdfTextExtractor.GetTextFromPage(page, strategy);

                pdfStringBuilder.Append(text);
                posibleTitles.AddRange(strategy.GetPosibleTitles());
            }

            // TODO: Eliminar pie de pagina
            string markdown = ConvertToMarkdown(
                pdfStringBuilder.ToString(),
                posibleTitles);

            var documents = await _markdownProcessor.ProcessAsync(
                path,
                Encoding.UTF8.GetBytes(markdown));

            return null!;
        }

        static string ConvertToMarkdown(
            string pdfText,
            List<string> posibleTitles)
        {
            var lines = pdfText.Split(
                "\n",
                StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);

            // Primera linea es el titulo
            var result = new StringBuilder();

            // Se convierten titulos a titulos de md
            foreach (var line in lines)
            {
                if (TitleRegex().IsMatch(line) &&
                    !IndexRegex().IsMatch(line) &&
                    posibleTitles.Contains(line))
                {
                    // Añade hashtags dependiendo de la cantidad de puntos
                    int puntos = line.Count(c => c == '.');

                    string mdTitulo = Enumerable.Range(0, puntos - 1)
                        .Aggregate(
                            $"# {TitleRegex().Replace(line, string.Empty)}",
                            (acc, _) => $"#{acc}");

                    result.AppendLine(mdTitulo);
                }
                else
                {
                    result.AppendLine(line);
                }
            }

            return result.ToString();
        }

        [GeneratedRegex(@"^[1-9]\d*(\.\d+)*\.\s+")]
        private static partial Regex TitleRegex();

        [GeneratedRegex(@"\b\w+(\s*\.\s*)+\d+\b$")]
        private static partial Regex IndexRegex();
    }
}
