using AppServices.Abstractions;
using AppServices.Helpers;
using Domain.Abstractions.Factories;
using Domain.Entities.DocumentoAgregado;
using iText.Forms.Xfdf;
using Markdig;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using ReverseMarkdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.DocumentProcessing
{
    internal class MarkdownProcessor(IDocumentFactory _documentFactory)
        : IDocumentProcessor
    {
        public Task<Document> ProcessAsync(string path, byte[] documentData)
        {
            string md = Encoding.UTF8.GetString(documentData);

            var chunks = ChunkMarkdown(md);

            return _documentFactory.CreateAsync(path, md, chunks);
        }

        static List<string> ChunkMarkdown(string markdown)
        {
            // Parse the markdown using Markdig
            var document = Markdown.Parse(markdown, trackTrivia: true);

            // Elimina los saltos de linea que no son parte de una tabla
            // Asi se evitan parrafos con saltos de linea en medio
            document.Where(x => x is LeafBlock)
                .Cast<LeafBlock>()
                .SelectMany(x => x.Inline?.Descendants<LineBreakInline>() ?? [])
                .ToList()
                .ForEach(
                    bl =>
                    {
                        var parent = bl.Parent;

                        if (bl.NextSibling is LiteralInline nextSibling)
                        {
                            string nextSiblingText = nextSibling.Content
                                .ToString();

                            // Si es new line de una tabla, no lo elimina
                            if (nextSiblingText.StartsWith('|') &&
                                nextSiblingText.EndsWith('|'))
                            {
                                return;
                            }
                        }

                        if (bl.PreviousSibling is LiteralInline previousSibling)
                        {
                            string previousSiblingText = previousSibling.Content
                                .ToString();

                            // Si es new line de una tabla, no lo elimina
                            if (previousSiblingText.StartsWith('|') &&
                                previousSiblingText.EndsWith('|'))
                            {
                                return;
                            }
                        }

                        bl.ReplaceBy(new LiteralInline(" "));

                        bl.Remove();
                    });

            // Elimina los tematic breaks
            document.Descendants<ThematicBreakBlock>()
                .ToList()
                .ForEach(
                    tb =>
                    {
                        document.Remove(tb);
                    });

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
                        currentChunkHeaders.Add(header);
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
            var writer = new StringWriter();

            var renderer = new NormalizeRenderer(writer);

            renderer.Write(node);

            return writer.ToString().Replace("\t\t", "\t");
        }

        /// <summary>
        /// Función auxiliar para extraer texto de HeadingBlock
        /// </summary>
        /// <param name="heading">Encabezado</param>
        /// <returns>Texto del encabezado</returns>
        static string GetHeadingText(HeadingBlock heading)
        {
            var writer = new StringWriter();

            new NormalizeRenderer(writer).Render(heading);

            return writer.ToString();
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

            var currentChunkTextString = currentChunkText.ToString().Trim();

            if (currentChunkTextString.Length > 0)
            {
                chunk.AppendLine();
                chunk.Append(currentChunkTextString);
            }
            else
            {
                return null;
            }

            return chunk.ToString()
                .Trim()
                .Replace("\t", string.Empty)
                .EliminarEspaciosInnecesarios()
                .EliminarNewLinesInnecesarias();
        }
    }
}
