using AppServices.Abstractions;
using AppServices.Helpers;
using AppServices.Ports;
using Domain.Entities;
using HtmlAgilityPack;
using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Extensions.Tables;
using Markdig.Renderers;
using Markdig.Renderers.Normalize;
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.Extensions.DependencyInjection;
using ReverseMarkdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.XPath;
using Ude;

namespace AppServices.DocumentProcessing
{
    internal class HtmlProcessor(
        Func<ICharsetDetector> charsetDetectorFactory,
        [FromKeyedServices(".md")]IDocumentProcessor _markdowProcessor)
        : IDocumentProcessor
    {
        public Task<List<Document>> ProcessAsync(
            string path,
            byte[] documentData)
        {
            var charsetDetector = charsetDetectorFactory.Invoke();
            charsetDetector.Feed(documentData, 0, documentData.Length);
            charsetDetector.DataEnd();

            string html = Encoding
                .GetEncoding(charsetDetector.Charset)
                .GetString(documentData)
                .Replace("&nbsp;", string.Empty);

            var mdConverterConfig = new Config
            {
                CleanupUnnecessarySpaces = true,
                RemoveComments = true,
                UnknownTags = Config.UnknownTagsOption.Bypass,
                SuppressDivNewlines = true,
                SmartHrefHandling = true,
                WhitelistUriSchemes = ["/", "../"],
                TableHeaderColumnSpanHandling = false,
            };

            var converter = new Converter(mdConverterConfig);
            string md = converter.Convert(html);

            string cleanMd = CleanMarkdown(md);

            return _markdowProcessor.ProcessAsync(
                path,
                Encoding.UTF8.GetBytes(cleanMd));
        }

        /// <summary>
        /// Limpia el markdown de elementos innecesarios.
        /// </summary>
        /// <param name="md">Documento markdown</param>
        /// <returns>Markdown limpio</returns>
        static string CleanMarkdown(string md)
        {
            // Para normalizar las listas
            md = md.Replace("§", "-");

            // Elimina los bloques de citas
            var mdLines = md.ReadLines()
                .Select(
                    line =>
                    {
                        string tmp = line;

                        while (tmp.StartsWith('>'))
                        {
                            tmp = tmp.Remove(0, 1).TrimStart();
                        }

                        return tmp;
                    })
                .ToList();

            // Transforma el titulo de la tabla en un header 1
            if (mdLines.First().StartsWith("|  |"))
            {
                int endTableIndex = mdLines.FindIndex(
                    line => line.Contains("| --- |"));

                StringBuilder titleBuilder = new();
                titleBuilder.Append("# ");

                mdLines.Take(endTableIndex)
                    .ToList()
                    .ForEach(
                        line =>
                        {
                            titleBuilder.Append(
                                line.Replace("|", string.Empty).Trim());
                        });

                mdLines.RemoveRange(1, endTableIndex);

                mdLines[0] = titleBuilder.ToString()
                    .Replace("\r\n", string.Empty)
                    .Replace("\t", string.Empty)
                    .Trim();
            }

            // Elimina pie de pagina de contacto
            int footerIndex = mdLines.FindIndex(
                line => line.Contains("|  | CAPATAZ en YouTube | |"));

            if (footerIndex != -1)
            {
                mdLines.RemoveRange(footerIndex, mdLines.Count - footerIndex);
            }

            StringBuilder stringBuilder = new();
            stringBuilder.AppendJoin("\r\n", mdLines);

            return stringBuilder.ToString();
        }
    }
}
