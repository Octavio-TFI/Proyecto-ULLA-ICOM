using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace AppServices.DocumentProcessing.Tests
{
    internal class PdfProcessorTests
    {
        [Test]
        public async Task ProcessAsyncTest()
        {
            // Arrange
            string path = "path";

            var pdfStream = new MemoryStream();
            var pdfWriter = new PdfWriter(pdfStream);
            var pdf = new PdfDocument(pdfWriter);
            var doc = new iText.Layout.Document(pdf);

            doc.Add(new Paragraph("Titulo").SetFontSize(16));
            doc.Add(new Paragraph("Texto Titulo ").SetFontSize(12));
            doc.Add(new Paragraph("Mas Texto Titulo").SetFontSize(12));

            doc.Add(new Paragraph("Contenidos").SetFontSize(15));
            doc.Add(new Paragraph("1. Titulo 1. ....... 1").SetFontSize(12));
            doc.Add(new Paragraph("1.1. Titulo 1.1. ....... 1").SetFontSize(12));

            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            doc.Add(new Paragraph("1. Titulo 1.").SetFontSize(15));
            doc.Add(new Paragraph("Texto titulo 1.").SetFontSize(12));
            doc.Add(new Paragraph("1.1. Titulo 1.1.").SetFontSize(15));

            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            doc.Add(new Paragraph("2. Titulo 2.").SetFontSize(15));
            doc.Add(new Paragraph("Texto titulo 2.").SetFontSize(12));
            doc.Add(new Paragraph("2.1. Titulo 2.1.").SetFontSize(15));

            doc.Add(new Paragraph("Contacto").SetFontSize(15));
            doc.Add(new Paragraph("email"));

            doc.Close();

            var markdownProcessorMock = new Mock<IDocumentProcessor>();
            markdownProcessorMock.Setup(
                x => x.ProcessAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
                .ReturnsAsync(
                    (string path, byte[] data) =>
                    {
                        return new Document
                        {
                            Filename = path,
                            Texto = Encoding.UTF8.GetString(data)
                        };
                    });

            var pdfProcessor = new PdfProcessor(markdownProcessorMock.Object);

            // Act
            var result = await pdfProcessor.ProcessAsync(
                path,
                pdfStream.ToArray());

            // Assert
            Assert.That(result.Filename, Is.EqualTo(path));
            Assert.That(
                result.Texto,
                Is.EqualTo(
                    @"# Titulo
Texto Titulo Mas Texto Titulo
## Titulo 1.
Texto titulo 1.
### Titulo 1.1.
## Titulo 2.
Texto titulo 2.
### Titulo 2.1.
"));
        }
    }
}
