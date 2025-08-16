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

        [Test]
        public async Task LlamarHerramientaAsync_DevuelveMensajeHerramienta()
        {
            // Arrange
            var kernelBuilder = Kernel.CreateBuilder();
            var kernel = kernelBuilder.Build();
            kernel.Plugins.AddFromObject(new TestHerramientas(), "TestPlugin");

            var agent = new ChatCompletionAgent() { Kernel = kernel };
            var agente = new Agent(
                agent,
                new Mock<IChatHistoryAdapter>().Object);

            var llamada = new MensajeLlamadaHerramienta
            {
                PluginName = "TestPlugin",
                FunctionName = "TestFunction",
                Argumentos =
                    new Dictionary<string, object?> { ["param"] = "valor" },
                DateTime = DateTime.UtcNow
            };

            // Act
            var mensajeHerramienta = await agente.LlamarHerramientaAsync(
                llamada);

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(
                        mensajeHerramienta,
                        Is.TypeOf<MensajeHerramientaInfo>());
                    Assert.That(
                        mensajeHerramienta.Llamada.FunctionName,
                        Is.EqualTo("TestFunction"));
                    Assert.That(
                        ((MensajeHerramientaInfo)mensajeHerramienta).DocumentosRecuperados,
                        Is.Empty);
                });
        }

        [Test]
        public void LlamarHerramientaAsync_RetornoInvalido_LanzaExcepcion()
        {
            // Arrange
            var kernelBuilder = Kernel.CreateBuilder();
            var kernel = kernelBuilder.Build();
            kernel.Plugins.AddFromObject(new TestHerramientas(), "TestPlugin");

            var agent = new ChatCompletionAgent() { Kernel = kernel };
            var agente = new Agent(
                agent,
                new Mock<IChatHistoryAdapter>().Object);

            var llamada = new MensajeLlamadaHerramienta
            {
                PluginName = "TestPlugin",
                FunctionName = "BadFunction",
                Argumentos = new Dictionary<string, object?>(),
                DateTime = DateTime.UtcNow
            };

            // Act & Assert
            Assert.ThrowsAsync<Exception>(
                async () => await agente.LlamarHerramientaAsync(llamada));
        }

        private sealed class TestHerramientas
        {
            [KernelFunction]
            public MensajeHerramientaInfo TestFunction(string param)
            {
                return new MensajeHerramientaInfo([], [])
                {
                    Llamada =
                        new MensajeLlamadaHerramienta
                        {
                            PluginName = "TestPlugin",
                            FunctionName = "TestFunction",
                            Argumentos =
                                new Dictionary<string, object?>
                                    {
                                        ["param"] = param
                                    },
                            DateTime = DateTime.UtcNow
                        },
                    DateTime = DateTime.UtcNow
                };
            }

            [KernelFunction]
            public string BadFunction()
            {
                return null!;
            }
        }
    }
}
