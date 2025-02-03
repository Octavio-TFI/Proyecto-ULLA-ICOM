using AppServices.Abstractions;
using AppServices.Ports;
using Domain.Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.XPath;
using Ude;

namespace AppServices.DocumentProcessors
{
    internal class HtmlDocumentProcessor(
        IFileManager _fileManager,
        IDocumentFactory _documentFactory)
        : IDocumentProcessor
    {
        public async Task<List<Document>> ProcessAsync(string path)
        {
            var bytes = await _fileManager.ReadAllBytesAsync(path);

            var charsetDetector = new CharsetDetector();
            charsetDetector.Feed(bytes, 0, bytes.Length);
            charsetDetector.DataEnd();

            string html = Encoding
                .GetEncoding(charsetDetector.Charset)
                .GetString(bytes);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//body");

            // Elimina tabla en el pie de la pagina
            htmlBody.SelectNodes("//table")?.LastOrDefault()?.Remove();

            var htmlBodyText = HttpUtility.HtmlDecode(htmlBody.InnerText);

            var parrafos = htmlBodyText
                .Split(
                    ['\r', '\n'],
                    StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries)
                .Except(["Buscando módulos de Ayuda de CAPATAZ..."])
                .ToArray();

            if (parrafos.Length == 0)
            {
                _fileManager.Delete(path);

                return [];
            }

            int endOfPage = Array.IndexOf(parrafos, "Vea también");

            if (endOfPage == -1)
            {
                endOfPage = Array.IndexOf(parrafos, "Ver también");
            }

            if (endOfPage == -1)
            {
                endOfPage = parrafos.Length;
            }

            List<Document> documents = [];

            // Primer parrafo es el titulo
            var parentDocument = await _documentFactory.CreateAsync(
                path,
                parrafos[0]);

            documents.Add(parentDocument);

            foreach (string parrafo in parrafos[1..endOfPage])
            {
                var child = await _documentFactory.CreateAsync(
                    path,
                    parrafo,
                    parentDocument);

                documents.Add(child);
            }


            return documents;
        }
    }
}
