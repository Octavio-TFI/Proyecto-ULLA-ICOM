using Microsoft.Extensions.DependencyInjection;
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

            chatHistoryFactoryMock
                .Setup(x => x.Create(mensajes))
                .ReturnsAsync(chatHistory);

            chatCompletionMock
                .Setup(
                    x => x.GetChatMessageContentsAsync(
                        chatHistory,
                        It.Is<PromptExecutionSettings>(
                            s => s.FunctionChoiceBehavior is AutoFunctionChoiceBehavior),
                        kernel,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    [new ChatMessageContent(AuthorRole.Assistant, "AI response")]);

            mensajeFactoryMock.Setup(
                x => x.CreateMensajeTextoGenerado(1, "AI response"))
                .Returns(mensajeGenerado);

            var generadorRespuesta = new GeneradorRespuesta(
                kernel,
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
