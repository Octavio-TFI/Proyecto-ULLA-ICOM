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
        public async Task<List<Document>> ProcessAsync(
            string path,
            byte[] documentData)
        {
            using MemoryStream pdfStream = new(documentData);

            using var pdfReader = new PdfReader(
                pdfStream,
                new ReaderProperties { });
            using var pdf = new PdfDocument(pdfReader);

            var pdfStringBuilder = new StringBuilder();

            for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
            {
                var page = pdf.GetPage(i);
                var a = page.GetAnnotations();
                var text = PdfTextExtractor.GetTextFromPage(page);

                pdfStringBuilder.Append(text);
            }
            // TODO: Eliminar pie de pagina
            // TODO: Numeracion de imagenes a veces se toman como titulo
            // Para esto implementar un ITextExtractionStrategy para identificar los titulos con el formato del texto.
            string markdown = ConvertToMarkdown(pdfStringBuilder.ToString());

            var documents = await _markdownProcessor.ProcessAsync(
                path,
                Encoding.UTF8.GetBytes(markdown));

            return [];
        }

        static string ConvertToMarkdown(string pdfText)
        {
            var lines = pdfText.Split(
                "\n",
                StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);

            // Primera linea es el titulo
            var result = new StringBuilder();

            foreach (var line in lines)
            {
                if (TitleRegex().IsMatch(line) && !IndexRegex().IsMatch(line))
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
