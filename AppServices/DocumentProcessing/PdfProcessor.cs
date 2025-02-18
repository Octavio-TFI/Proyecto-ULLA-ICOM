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
        public Task<Document> ProcessAsync(string path, byte[] documentData)
        {
            using MemoryStream pdfStream = new(documentData);

            using var pdfReader = new PdfReader(
                pdfStream,
                new ReaderProperties { });
            using var pdf = new PdfDocument(pdfReader);

            var pdfStringBuilder = new StringBuilder();

            for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
            {
                // TODO: Mejorar extraccion de listas de items
                var strategy = new PdfToMdTextExtractionStrategy();

                var page = pdf.GetPage(i);
                var text = PdfTextExtractor.GetTextFromPage(page, strategy);

                pdfStringBuilder.Append(text);
            }

            string md = CleanMd(pdfStringBuilder.ToString());

            return _markdownProcessor.ProcessAsync(
                path,
                Encoding.UTF8.GetBytes(md));
        }

        static string CleanMd(string md)
        {
            md = PaginaRegex().Replace(md, string.Empty);
            md = UltimaActualizacionRegex().Replace(md, string.Empty);
            md = UrlRegex().Replace(md, string.Empty);

            var lines = md.Split(
                "\n",
                StringSplitOptions.TrimEntries |
                    StringSplitOptions.RemoveEmptyEntries);

            var result = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // Elimina indice
                if (line == "Contenidos")
                {
                    continue;
                }

                if (line.Contains("..."))
                {
                    continue;
                }

                // Elimina sección de contacto
                if (line == "Contacto")
                {
                    break;
                }

                // Se convierte la primera linea en titulo
                if (result.Length == 0)
                {
                    line = $"# {line}";
                }

                result.AppendLine(line);
            }

            return result.ToString();
        }

        [GeneratedRegex(@"Página\s+nº\s+(\d+)\s+de\s+(\d+)")]
        private static partial Regex PaginaRegex();

        [GeneratedRegex(
            @"Última actualización \d{1,2}/\d{1,2}/(\d{2}(?:\d{2})?) \d{1,2}:\d{2}:\d{2}")]
        private static partial Regex UltimaActualizacionRegex();

        [GeneratedRegex(@"(?:www\.)?[a-zA-Z0-9-]+\.(com\.ar)")]
        private static partial Regex UrlRegex();
    }
}
