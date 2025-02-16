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

            for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
            {
                var strategy = new PdfToMdTextExtractionStrategy();

                var page = pdf.GetPage(i);
                var text = PdfTextExtractor.GetTextFromPage(page, strategy);

                pdfStringBuilder.Append(text);
            }

            string md = CleanMd(pdfStringBuilder.ToString());

            var documents = await _markdownProcessor.ProcessAsync(
                path,
                Encoding.UTF8.GetBytes(md));

            return null!;
        }

        static string CleanMd(string md)
        {
            var lines = md.Split(
                "\n",
                StringSplitOptions.TrimEntries |
                    StringSplitOptions.RemoveEmptyEntries);

            var result = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // Se convierte la primera linea en titulo
                if (i == 0)
                {
                    line = $"# {line}";
                }

                // Se elimina indice
                if (line == "Contenidos")
                {
                    continue;
                }

                if (line.Contains("..."))
                {
                    continue;
                }

                // Se elimina sección de contacto
                if (line == "Contacto")
                {
                    break;
                }

                result.AppendLine(line);
            }

            return result.ToString();
        }
    }
}
