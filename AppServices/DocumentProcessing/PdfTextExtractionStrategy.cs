using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.DocumentProcessing
{
    internal class PdfTextExtractionStrategy()
        : SimpleTextExtractionStrategy
        , ITextExtractionStrategy
    {
        readonly IList<string> posibleTitles = [];

        readonly StringBuilder currentLine = new();
        float currentLinePosition;
        float currentLineSize;

        public override void EventOccurred(IEventData data, EventType type)
        {
            base.EventOccurred(data, type);

            if (!type.Equals(EventType.RENDER_TEXT))
            {
                return;
            }

            TextRenderInfo textRenderInfo = (TextRenderInfo)data;
            LineSegment lineSegment = textRenderInfo.GetBaseline();
            float linePosition = lineSegment.GetStartPoint().Get(1);

            if (currentLinePosition != linePosition)
            {
                if (currentLine.Length > 0)
                {
                    if (currentLineSize > 12)
                    {
                        posibleTitles.Add(currentLine.ToString().Trim());
                    }
                }

                currentLine.Clear();
                currentLinePosition = linePosition;
                currentLineSize = 0;
            }

            float fontSize = textRenderInfo.GetFontSize();
            Vector sizeHighVector = new(0, fontSize, 0);
            Matrix matrix = textRenderInfo.GetTextMatrix();
            float fontSizeAdjusted = sizeHighVector.Cross(matrix).Length();

            if (fontSizeAdjusted > currentLineSize)
            {
                currentLineSize = fontSizeAdjusted;
            }

            currentLine.Append(textRenderInfo.GetText());
        }

        public IReadOnlyList<string> GetPosibleTitles()
        {
            return posibleTitles.AsReadOnly();
        }
    }
}
