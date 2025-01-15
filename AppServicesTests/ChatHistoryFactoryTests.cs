using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Tests
{
    internal class ChatHistoryFactoryTests
    {
        [Test]
        public async Task CreateAsyncTask()
        {
            // Arrange
            List<Mensaje> mensajes =
            [new MensajeTexto()
            {
                Texto = "0",
                DateTime = DateTime.Now,
                ChatId = 1,
                Tipo = TipoMensaje.Usuario
            },new MensajeTexto()
            {
                Texto = "1",
                DateTime = DateTime.Now,
                ChatId = 1,
                Tipo = TipoMensaje.Asistente
            }];

            var fileManager = new Mock<IFileManager>();
            fileManager.Setup(x => x.ReadAllTextAsync(It.IsAny<string>()))
                .ReturnsAsync("systemPrompt");
            var chatHistoryFactory = new ChatHistoryFactory(fileManager.Object);

            // Act
            var chatHistory = await chatHistoryFactory.CreateAsync(mensajes);

            // Assert
            Assert.That(chatHistory.First().Role, Is.EqualTo(AuthorRole.System));
            Assert.That(
                chatHistory.First().ToString(),
                Is.EqualTo("systemPrompt"));
            Assert.That(chatHistory[1].Role, Is.EqualTo(AuthorRole.User));
            Assert.That(chatHistory[1].ToString(), Is.EqualTo("0"));
            Assert.That(chatHistory[2].Role, Is.EqualTo(AuthorRole.Assistant));
            Assert.That(chatHistory[2].ToString(), Is.EqualTo("1"));
        }
    }
}
