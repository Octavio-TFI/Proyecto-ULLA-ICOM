using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Tests
{
    internal class GeneradorRespuestaTests
    {
        [Test]
        public async Task GenerarRespuestaAsyncTask()
        {
            // Arrange
            List<Mensaje> mensajes = [new MensajeTexto()
            {
                Texto = "Hola",
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Usuario,
                ChatId = 1
            }];

            var chatHistory = new ChatHistory();

            var mensajeGenerado = new MensajeTexto()
            {
                Texto = "AI response",
                DateTime = DateTime.Now,
                Tipo = TipoMensaje.Asistente,
                ChatId = 1
            };

            var chatCompletionMock = new Mock<IChatCompletionService>();
            var mensajeFactoryMock = new Mock<IMensajeFactory>();
            var chatHistoryFactoryMock = new Mock<IChatHistoryFactory>();

            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton(chatCompletionMock.Object);

            var kernel = kernelBuilder.Build();

            var agent = new ChatCompletionAgent() { Kernel = kernel };

            chatHistoryFactoryMock
                .Setup(x => x.Create(mensajes))
                .Returns(chatHistory);

            chatCompletionMock
                .Setup(
                    x => x.GetChatMessageContentsAsync(
                        chatHistory,
                        It.IsAny<PromptExecutionSettings>(),
                        kernel,
                        default))
                .ReturnsAsync(
                    new List<ChatMessageContent>
                    {
                        new(AuthorRole.Assistant, "AI response")
                    }.AsReadOnly());

            mensajeFactoryMock.Setup(
                x => x.CreateMensajeTextoGenerado(1, "AI response"))
                .Returns(mensajeGenerado);

            var generadorRespuesta = new GeneradorRespuesta(
                agent,
                mensajeFactoryMock.Object,
                chatHistoryFactoryMock.Object);

            // Act
            var result = await generadorRespuesta.GenerarRespuestaAsync(
                mensajes);

            // Assert
            Assert.That(result, Is.EqualTo(mensajeGenerado));
        }
    }
}
