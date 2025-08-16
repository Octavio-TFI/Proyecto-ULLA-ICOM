using Domain.Entities.ChatAgregado;
using Domain.ValueObjects;
using Infrastructure.LLM.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.Tests
{
    internal class AgentTests
    {
        [Test]
        public async Task GenerarRespuesta_ListaDeMensajesAsync()
        {
            // Arrange
            List<Mensaje> mensajes = [new MensajeTextoUsuario()
            {
                Texto = "Hola",
                DateTime = DateTime.Now
            }];

            Dictionary<string, object?> arguments = new()
            {
                ["argument"] = "system"
            };

            var chatHistory = new ChatHistory();

            string expectedResponse = "AI response";
            var expectedFunctionCalls = new List<AgentFunctionCall>
            {
                new()
                {
                    PluginName = "TestPlugin",
                    FunctionName = "TestFunction",
                    Arguments =
                        new Dictionary<string, object?> { ["param"] = "value" }
                }
            };

            var chatCompletionMock = new Mock<IChatCompletionService>();
            var chatHistoryFactoryMock = new Mock<IChatHistoryAdapter>();
            var toolExtractorMock = new Mock<IToolCallExtractor>();

            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton(chatCompletionMock.Object);
            kernelBuilder.Services.AddSingleton(toolExtractorMock.Object);

            var kernel = kernelBuilder.Build();

            var agent = new ChatCompletionAgent()
            {
                Kernel = kernel,
                Instructions = "{{$argument}}"
            };

            var chatMessageContent = new ChatMessageContent(
                AuthorRole.Assistant,
                expectedResponse);

            chatHistoryFactoryMock
                .Setup(x => x.AdaptAsync(mensajes))
                .ReturnsAsync(chatHistory);

            chatCompletionMock
                .Setup(
                    x => x.GetChatMessageContentsAsync(
                        It.Is<ChatHistory>(c => c.First().Content == "system"),
                        It.IsAny<PromptExecutionSettings>(),
                        kernel,
                        default))
                .ReturnsAsync(
                    new List<ChatMessageContent> { chatMessageContent }.AsReadOnly(
                        ));

            toolExtractorMock
                .Setup(x => x.Extract(chatMessageContent))
                .Returns(expectedFunctionCalls);

            var generadorRespuesta = new Agent(
                agent,
                chatHistoryFactoryMock.Object);

            // Act
            var result = await generadorRespuesta
                .GenerarRespuestaAsync(mensajes, arguments)
                .ConfigureAwait(false);

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(result.Texto, Is.EqualTo(expectedResponse));
                    Assert.That(result.FunctionCalls, Has.Count.EqualTo(1));
                    Assert.That(
                        result.FunctionCalls[0].PluginName,
                        Is.EqualTo("TestPlugin"));
                    Assert.That(
                        result.FunctionCalls[0].FunctionName,
                        Is.EqualTo("TestFunction"));
                });
        }

        [Test]
        public async Task GenerarRespuesta_MensajeUnicoAsync()
        {
            // Arrange
            string mensaje = "Hola";
            string expectedResponse = "AI response";
            var expectedFunctionCalls = new List<AgentFunctionCall>();

            var chatCompletionMock = new Mock<IChatCompletionService>();
            var chatHistoryFactoryMock = new Mock<IChatHistoryAdapter>();
            var toolExtractorMock = new Mock<IToolCallExtractor>();

            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton(chatCompletionMock.Object);
            kernelBuilder.Services.AddSingleton(toolExtractorMock.Object);

            var kernel = kernelBuilder.Build();

            var agent = new ChatCompletionAgent() { Kernel = kernel };

            var chatMessageContent = new ChatMessageContent(
                AuthorRole.Assistant,
                expectedResponse);

            chatCompletionMock
                .Setup(
                    x => x.GetChatMessageContentsAsync(
                        It.Is<ChatHistory>(
                            c => c.Any(m => m.ToString() == mensaje)),
                        It.IsAny<PromptExecutionSettings>(),
                        kernel,
                        default))
                .ReturnsAsync(
                    new List<ChatMessageContent> { chatMessageContent }.AsReadOnly(
                        ));

            toolExtractorMock
                .Setup(x => x.Extract(chatMessageContent))
                .Returns(expectedFunctionCalls);

            var generadorRespuesta = new Agent(
                agent,
                chatHistoryFactoryMock.Object);

            // Act
            var result = await generadorRespuesta
                .GenerarRespuestaAsync(mensaje)
                .ConfigureAwait(false);

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(result.Texto, Is.EqualTo(expectedResponse));
                    Assert.That(result.FunctionCalls, Has.Count.EqualTo(0));
                });
        }

        // TODO: Añadir tests para llamar herramienta
    }
}
