using AppServices.Abstractions;
using AppServices.Ports;
using Domain.Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
            if (path.Contains("(ext)"))
            {
                return [];
            }

            var bytes = await _fileManager.ReadAllBytesAsync(path);

            var charsetDetector = new CharsetDetector();
            charsetDetector.Feed(bytes, 0, bytes.Length);
            charsetDetector.DataEnd();

            string html = Encoding
                .GetEncoding(charsetDetector.Charset)
                .GetString(bytes);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var htmlBodyText = htmlDoc.DocumentNode.SelectSingleNode("//body")
                .InnerText;

            var parrafos = HttpUtility.HtmlDecode(htmlBodyText)
                .Split(
                    ['\r', '\n'],
                    StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries)
                .ToArray();

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
