using AppServices.Abstractions;
using AppServices.Helpers;
using AppServices.Ports;
using Domain.Entities;
using HtmlAgilityPack;
using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Renderers.Normalize;
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using ReverseMarkdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
                .GetString(bytes)
                .Replace("&nbsp;", string.Empty);

            var mdConverterConfig = new ReverseMarkdown.Config
            {
                CleanupUnnecessarySpaces = true,
                RemoveComments = true,
                UnknownTags = ReverseMarkdown.Config.UnknownTagsOption.Bypass,
                SuppressDivNewlines = true,
                SmartHrefHandling = true,
                WhitelistUriSchemes = ["/", "../"],
                TableHeaderColumnSpanHandling = false,
            };

            var converter = new ReverseMarkdown.Converter(mdConverterConfig);
            string md = converter.Convert(html);

            string cleanMd = CleanMarkdown(md);

            var chunks = ChunkMarkdown(cleanMd);

            List<Document> documents = [];

            foreach (var chunk in chunks)
            {
                var document = await _documentFactory.CreateAsync(path, chunk);

                documents.Add(document);
            }

            return documents;
        }

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

        public static List<string> ChunkMarkdown(string markdown)
        {
            // Parse the markdown using Markdig
            var document = Markdown.Parse(markdown);

            List<string> chunks = [];
            List<string> currentChunkHeaders = [];
            StringBuilder currentChunkText = new();
            // For maintaining heading hierarchy
            Stack<string> headingStack = new();

            foreach (var node in document)
            {
                if (node is HeadingBlock heading)
                {
                    string headingText = GetHeadingText(heading);

                    // When we hit a new heading, finalize the current chunk
                    if (currentChunkHeaders.Count > 0)
                    {
                        string? chunck = BuildChunk(
                            currentChunkHeaders,
                            currentChunkText);

                        if (chunck is not null)
                        {
                            chunks.Add(chunck);
                        }

                        currentChunkHeaders.Clear();
                        currentChunkText.Clear();
                    }

                    // Add the current heading to the stack, depending on the level
                    while (headingStack.Count >= heading.Level)
                    {
                        headingStack.Pop(); // Pop until we reach the current heading level
                    }

                    while (headingStack.Count + 1 < heading.Level)
                    {
                        headingStack.Push(string.Empty);
                    }

                    headingStack.Push(headingText);

                    // Add context (all headings in the stack) to the new chunk
                    foreach (var header in headingStack.Reverse())
                    {
                        currentChunkHeaders.Add($"{header}");
                    }
                }
                else
                {
                    // Append non-heading content to the current chunk
                    string nodeText = GetNodeText(node);
                    currentChunkText.Append(nodeText);
                }
            }

            // Add any remaining chunk at the end
            if (currentChunkHeaders.Count > 0)
            {
                string? chunck = BuildChunk(
                    currentChunkHeaders,
                    currentChunkText);

                if (chunck is not null)
                {
                    chunks.Add(chunck);
                }
            }

            return chunks;
        }

        /// <summary>
        /// Función auxiliar para extraer texto de nodo
        /// </summary>
        /// <param name="node">Nodo</param>
        /// <returns>Texto del nodo</returns>
        static string GetNodeText(MarkdownObject node)
        {
            var content = new StringBuilder();

            if (node is ParagraphBlock paragraph)
            {
                if (paragraph.Inline is not null)
                {
                    content.Append(GetNodeText(paragraph.Inline));

                    content.AppendLine();
                }
            }
            else if (node is ListBlock listBlock)
            {
                int i = 1;

                foreach (var listItem in listBlock)
                {
                    if (listItem is ListItemBlock listItemBlock)
                    {
                        foreach (var subNode in listItemBlock)
                        {
                            string subNodeText = GetNodeText(subNode);

                            content.Append($"{i}. {subNodeText}");

                            i++;
                        }
                    }
                }

                content.AppendLine();
            }
            else if (node is ContainerInline containerInline)
            {
                foreach (var inlineChild in containerInline)
                {
                    string text = GetNodeText(inlineChild);
                    string trimmedText = text.Trim();

                    // Si es fila de tabla agregar new line en vez de espacio
                    if (trimmedText.Length > 1 &&
                        trimmedText.StartsWith('|') &&
                        trimmedText.EndsWith('|'))
                    {
                        content.Append(trimmedText);
                        content.AppendLine();
                    }
                    else
                    {
                        content.Append(text);
                    }
                }
            }
            else if (node is LiteralInline literal)
            {
                var text = literal.Content.Text
                    .AsSpan(literal.Content.Start, literal.Content.Length)
                    .Trim();

                content.Append(text);
                content.Append(' ');
            }
            else if (node is CodeBlock codeBlock)
            {
                content.AppendLine(codeBlock.Lines.ToString());
            }

            return content.ToString();
        }

        /// <summary>
        /// Función auxiliar para extraer texto de HeadingBlock
        /// </summary>
        /// <param name="heading">Encabezado</param>
        /// <returns>Texto del encabezado</returns>
        static string GetHeadingText(HeadingBlock heading)
        {
            StringBuilder headingText = new();

            if (heading.Inline is null)
            {
                return string.Empty;
            }

            foreach (var inline in heading.Inline)
            {
                headingText.Append(GetNodeText(inline));
            }

            return new StringBuilder()
                .Append('#', heading.Level)
                .Append(' ')
                .Append(headingText.ToString().Trim())
                .ToString();
        }

        /// <summary>
        /// Construye un chunck a partir de los headers y del texto del chunck
        /// actual
        /// </summary>
        /// <param name="currentChunkHeaders">Headers del chunck actual</param>
        /// <param name="currentChunkText">Texto del chunk actual</param>
        /// <returns>Chunck construido</returns>
        static string? BuildChunk(
            List<string> currentChunkHeaders,
            StringBuilder currentChunkText)
        {
            if (currentChunkHeaders.Any(
                h =>
                {
                    string header = h.ToLower().EliminarAcentos();

                    return header.Contains("ver tambien") ||
                        header.Contains("vea tambien");
                }))
            {
                return null;
            }

            // Elimina los headers vacios
            currentChunkHeaders = currentChunkHeaders.Select(
                h =>
                {
                    if (string.IsNullOrWhiteSpace(h.Replace("#", string.Empty)))
                    {
                        return null;
                    }

                    return h;
                })
                .Where(h => h is not null)
                .Cast<string>()
                .ToList();

            var chunk = new StringBuilder();
            chunk.AppendJoin("\r\n", currentChunkHeaders);

            if (currentChunkText.Length > 0)
            {
                chunk.AppendLine();
                chunk.Append(currentChunkText.ToString().Trim());
            }
            else
            {
                return null;
            }

            return chunk.ToString().Trim().EliminarEspaciosInnecesarios();
        }
    }
}
