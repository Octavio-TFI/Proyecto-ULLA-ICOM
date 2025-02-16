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
                var strategy = new PdfTextExtractionStrategy();

                var page = pdf.GetPage(i);
                var text = PdfTextExtractor.GetTextFromPage(page, strategy);

                pdfStringBuilder.Append(text);
            }

            string md = SetDocumentTitle(pdfStringBuilder.ToString());

            var documents = await _markdownProcessor.ProcessAsync(
                path,
                Encoding.UTF8.GetBytes(md));

            return null!;
        }

        static string SetDocumentTitle(string pdfText)
        {
            var lines = pdfText.Split(
                "\n",
                StringSplitOptions.TrimEntries |
                    StringSplitOptions.RemoveEmptyEntries);

            // Primera linea es el titulo
            var result = new StringBuilder();

            // Se convierten titulos a titulos de md
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (result.Length == 0)
                {
                    result.AppendLine($"# {line}");
                }

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
