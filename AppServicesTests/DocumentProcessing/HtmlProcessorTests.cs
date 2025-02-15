using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ude;

namespace AppServices.DocumentProcessing.Tests
{
    internal class HtmlProcessorTests
    {
        const string HTML = @"<html>
<body>
    <div id=""encabezadoPagina"">
	<table cellspacing=""5"" class=""tablaEncabezado"">
	    <tbody><tr>
		    <td class=""c1E""></td>
		    <td class=""c2E""><div id=""tituloPagina"">Test table title</div></td>
        </tr></tbody>
    </table>
    </div>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <p>§ Test item list</p>

    <h2>Test h2</h2>

    <blockquote>
        <p>Test blockquote</p>
    </blockquote>

    <a href=""../a.htm"" >URL Test</a>
    <a href=""/a.htm"" >URL Test 2</a>

    <h2>Ver también</h2>
    <h2>Ver tambien</h2>
    <h2>ver tambien</h2>
    <h2>Vea también</h2>
    <h2>Vea tambien</h2>
    <h2>vea tambien</h2>

    <p>Test ver tambien</p>

    <p>|  | CAPATAZ en YouTube | |</p>
    <p>|---|---|---|</p>
</body>
</html>";

        [Test]
        public async Task ProcessAsync_WhenCalled_ReturnsDocumentList()
        {
            // Arrange
            var path = "path";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var documentData = Encoding.Latin1.GetBytes(HTML);
            var charsetDetectorFactory = new Func<ICharsetDetector>(
                () => new CharsetDetector());
            var markdownProcessor = new Mock<IDocumentProcessor>();

            markdownProcessor.Setup(
                x => x.ProcessAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
                .ReturnsAsync(
                    (string mdPath, byte[] data) =>
                    {
                        return new Document()
                        {
                            Filename = mdPath,
                            Texto = Encoding.UTF8.GetString(data)
                        };
                    });

            var htmlProcessor = new HtmlProcessor(
                charsetDetectorFactory,
                markdownProcessor.Object);

            // Act
            var document = await htmlProcessor.ProcessAsync(path, documentData);

            // Assert
            Assert.That(document.Filename, Is.EqualTo(path));
            Assert.That(
                document.Texto,
                Is.EqualTo(
                    @"# Test table title

- Test item list

## Test h2



Test blockquote



    URL Test
    URL Test 2
"));
        }
    }
}
