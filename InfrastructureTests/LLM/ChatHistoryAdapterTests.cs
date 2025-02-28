using Infrastructure.LLM;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.Tests
{
    internal class ChatHistoryAdapterTests
    {
        [Test]
        public void Adapt()
        {
            // Arrange
            List<Mensaje> mensajes =
            [new MensajeTexto()
            {
                Texto = "0",
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Usuario
            },new MensajeTexto()
            {
                Texto = "1",
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Asistente
            }];


            var chatHistoryFactory = new ChatHistoryAdapter();

            // Act
            var chatHistory = chatHistoryFactory.Adapt(mensajes);

            // Assert
            Assert.That(chatHistory[0].Role, Is.EqualTo(AuthorRole.User));
            Assert.That(chatHistory[0].ToString(), Is.EqualTo("0"));
            Assert.That(chatHistory[1].Role, Is.EqualTo(AuthorRole.Assistant));
            Assert.That(chatHistory[1].ToString(), Is.EqualTo("1"));
        }
    }
}
