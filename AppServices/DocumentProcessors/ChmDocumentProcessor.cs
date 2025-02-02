using AppServices.Abstractions;
using AppServices.Ports;
using CHMsharp;
using Domain.Entities;
using HtmlAgilityPack;
using Microsoft.SemanticKernel.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Ude;

namespace AppServices.DocumentProcessors
{
    internal class ChmDocumentProcessor(
        IPathManager _pathManager,
        IDocumentFactory _documentFactory)
        : IDocumentProcessor
    {
        record ChmFileData(string Titulo, string[] Parrafos);

        public async Task<List<Document>> ProcessAsync(string path)
        {
            List<ChmFileData> chmFileDatas = [];

            ChmFile chmFile = ChmFile.Open(path);

            chmFile.Enumerate(
                EnumerateLevel.Normal,
                (ChmFile file, ChmUnitInfo ui, EnumeratorContext context) => EnumeratorCallback(
                    file,
                    ui,
                    context,
                    chmFileDatas),
                new EnumeratorContext());

            chmFile.Close();

            chmFileDatas = chmFileDatas.DistinctBy(
                chmFileData =>
                {
                    return new StringBuilder()
                        .Append(chmFileData.Titulo)
                        .AppendJoin("\r\n", chmFileData.Parrafos)
                        .ToString();
                })
                .ToList();

            List<Document> documents = [];

            foreach (var chmFileData in chmFileDatas)
            {
                var parentDocument = await _documentFactory.CreateAsync(
                    path,
                    chmFileData.Titulo);

                documents.Add(parentDocument);

                foreach (string parrafo in chmFileData.Parrafos)
                {
                    var child = await _documentFactory.CreateAsync(
                        path,
                        parrafo,
                        parentDocument);

                    documents.Add(child);
                }
            }

            return documents;
        }

        EnumerateStatus EnumeratorCallback(
            ChmFile file,
            ChmUnitInfo ui,
            EnumeratorContext context,
            List<ChmFileData> chmFileDatas)
        {
            if (_pathManager.GetExtension(ui.path) != ".htm")
                return EnumerateStatus.Continue;

            if (ui.length > 0)
            {
                var buf = new byte[ui.length];
                var ret = file.RetrieveObject(ui, ref buf, 0, buf.Length);

                if (ret > 0)
                {
                    var charsetDetector = new CharsetDetector();
                    charsetDetector.Feed(buf, 0, buf.Length);
                    charsetDetector.DataEnd();

                    var encoding = Encoding.GetEncoding(1252);

                    string html = Encoding
                        .GetEncoding(charsetDetector.Charset)
                        .GetString(buf);

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    var htmlBodyText = htmlDoc.DocumentNode
                        .SelectSingleNode("//body")
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
                    else if (endOfPage == -1)
                    {
                        endOfPage = parrafos.Length;
                    }

                    // Primer parrafo es el titulo
                    var chmFileData = new ChmFileData(
                        parrafos[0],
                        parrafos[1..endOfPage]);

                    chmFileDatas.Add(chmFileData);
                }
            }

            return EnumerateStatus.Continue;
        }
    }
}
