using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppServices.DocumentProcessing
{
    internal partial class PdfToMdTextExtractionStrategy()
        : ITextExtractionStrategy
    {
        readonly StringBuilder resultantText = new();

        readonly StringBuilder currentLine = new();
        float currentLinePosition;
        float currentLineFontSize;
        string? currentLineFont;

        [GeneratedRegex(@"^[1-9]\d*(\.\d+)*\.\s+")]
        private static partial Regex TitleRegex();

        public void EventOccurred(IEventData data, EventType type)
        {
            TextRenderInfo textRenderInfo = (TextRenderInfo)data;

            // Obtiene posicion y de la linea actual
            LineSegment lineSegment = textRenderInfo.GetBaseline();
            float linePosition = lineSegment.GetStartPoint().Get(1);

            // Obtiene el tamaño de la fuente
            float fontSize = textRenderInfo.GetFontSize();
            Vector sizeHighVector = new(0, fontSize, 0);
            Matrix matrix = textRenderInfo.GetTextMatrix();
            float fontSizeAdjusted = sizeHighVector.Cross(matrix).Length();

            // Obtiene la fuente actual
            string font = textRenderInfo.GetFont().GetFontProgram().ToString();

            // Si la linea actual esta en otra posicion, es una linea nueva
            if (linePosition != currentLinePosition)
            {
                string lineText = currentLine.ToString();

                // Si la linea actual tiene fuente distinta a la anterior, es un titulo
                if (fontSizeAdjusted != currentLineFontSize ||
                    font != currentLineFont ||
                    TitleRegex().IsMatch(lineText))
                {
                    // Si la linea actual tiene un tamaño de fuente mayor a 12 y
                    // es un titulo, se reemplaza por titulo de Markdown
                    if (currentLineFontSize > 12 &&
                        TitleRegex().IsMatch(lineText))
                    {
                        // Se reemplaza por titulo de Markdown
                        string mdTitulo = TransformToMdTitle(lineText);

                        resultantText.AppendLine(mdTitulo);
                    }
                    // Si no es titulo se agrega el texto tal cual en nueva linea
                    else
                    {
                        resultantText.AppendLine(lineText.TrimStart());
                    }
                }
                // Si la linea actual tiene el mismo tamaño de fuente y fuente
                // que la anterior son la misma
                else
                {
                    resultantText.Append(lineText);
                }

                currentLine.Clear();
                currentLinePosition = linePosition;
                currentLineFontSize = 0;
                currentLineFont = font;
            }

            if (fontSizeAdjusted > currentLineFontSize)
            {
                currentLineFontSize = fontSizeAdjusted;
            }

            currentLine.Append(textRenderInfo.GetText());
        }

        public string GetResultantText()
        {
            if (currentLine.Length > 0)
            {
                string lineText = currentLine.ToString();

                if (currentLineFontSize > 12 && TitleRegex().IsMatch(lineText))
                {
                    string mdTitulo = TransformToMdTitle(lineText);

                    resultantText.AppendLine(mdTitulo);
                }
                else
                {
                    resultantText.Append(lineText);
                }
            }

            return resultantText.ToString();
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return [EventType.RENDER_TEXT];
        }

        static string TransformToMdTitle(string lineText)
        {
            int puntos = TitleRegex().Match(lineText).Value.Count(c => c == '.');

            string mdTitulo = Enumerable.Range(0, puntos)
                .Aggregate(
                    $"# {TitleRegex().Replace(lineText, string.Empty)}",
                    (acc, _) => $"#{acc}");

            return mdTitulo.Trim();
        }
    }
}
