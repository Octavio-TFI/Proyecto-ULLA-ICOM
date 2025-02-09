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

            md = CleanMarkdown(md);

            var chunks = ChunkMarkdown(md);

            List<Document> documents = [];

            //// Primer parrafo es el titulo
            //var parentDocument = await _documentFactory.CreateAsync(
            //    path,
            //    parrafos[0]);

            //documents.Add(parentDocument);

            //foreach (string parrafo in parrafos[1..endOfPage])
            //{
            //    var child = await _documentFactory.CreateAsync(
            //        path,
            //        parrafo,
            //        parentDocument);

            //    documents.Add(child);
            //}

            return documents;
        }

        static string CleanMarkdown(string md)
        {
            // Para normalizar las listas
            md = md.Replace("§", "-");

            // Cambia el titulo de tabla a header 1
            bool tableFound = false;
            string titlePattern = @"\|[^\|]+\|[^\|]+\|";

            var mdLines = md.ReadLines();

            // Elimina los bloques de citas
            mdLines = mdLines
                .Select(
                    line =>
                    {
                        if (line.StartsWith('>'))
                        {
                            return line.Remove(0, 1).TrimStart();
                        }

                        return line;
                    });

            var titleLines = mdLines
                .Take(2)
                .Select(
                    line =>
                    {
                        // Check if we found the table with a title
                        if (!tableFound && Regex.IsMatch(line, titlePattern))
                        {
                            // Extract the title from the table (removing the '|')
                            string title = line.Replace("|", string.Empty)
                                .Trim();

                            tableFound = true;

                            return $"# {title}";
                        }
                        // Skip the next line if the table was found and contains the separator
                        else if (tableFound && line.Contains("| --- |"))
                        {
                            tableFound = false;  // Reset after skipping
                            return null;
                        }
                        else
                        {
                            return line;
                        }
                    })
                .Where(line => line is not null);

            StringBuilder stringBuilder = new();
            stringBuilder.AppendJoin("\r\n", titleLines);
            stringBuilder.AppendJoin("\r\n", mdLines.Skip(2));

            return stringBuilder.ToString();
        }

        public static List<string> ChunkMarkdown(string markdown)
        {
            // Parse the markdown using Markdig
            var document = Markdown.Parse(markdown);

            var thematicBreak = document.FirstOrDefault(
                x => x is ThematicBreakBlock);

            if (thematicBreak is not null)
            {
                int thematicBreakIndex = document.IndexOf(thematicBreak);

                // Remove all items after thematicBreak
                document.Skip(thematicBreakIndex)
                    .ToList()
                    .ForEach(x => document.Remove(x));
            }

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

                    // If the heading is empty, skip it
                    if (string.IsNullOrWhiteSpace(
                        headingText.Replace("#", string.Empty)))
                    {
                        continue;
                    }

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

            return chunk.ToString().Trim();
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
                foreach (var inline in paragraph.Inline)
                {
                    if (inline is LiteralInline literal)
                    {
                        content.Append(
                            literal.Content.Text
                                .AsSpan(
                                    literal.Content.Start,
                                    literal.Content.Length));
                        content.Append(' ');
                    }
                    else if (inline is ContainerInline containerInline)
                    {
                        foreach (var inlineChild in containerInline)
                        {
                            if (inlineChild is LiteralInline literalChild)
                            {
                                content.Append(
                                    literalChild.Content.Text
                                        .AsSpan(
                                            literalChild.Content.Start,
                                            literalChild.Content.Length));
                            }
                        }
                    }
                }

                // Add a new line at the end of the paragraph
                content.AppendLine();
            }
            else if (node is ListBlock listBlock)
            {
                foreach (var listItem in listBlock)
                {
                    if (listItem is ListItemBlock listItemBlock)
                    {
                        foreach (var subNode in listItemBlock)
                        {
                            string subNodeText = GetNodeText(subNode);

                            content.Append($"- {subNodeText}");
                        }
                    }
                }
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
                headingText.Append(inline.ToString());
            }

            return new StringBuilder()
                .Append('#', heading.Level)
                .Append(' ')
                .Append(headingText.ToString().Trim())
                .ToString();
        }
    }
}
