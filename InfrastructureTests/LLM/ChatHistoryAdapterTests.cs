using Domain.Abstractions;
using Domain.Entities.ChatAgregado;
using Infrastructure.LLM;
using Microsoft.SemanticKernel;
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
        private static ChatHistoryAdapter CreateChatHistoryAdapter()
        {
            var mockBuilder = new Mock<IMensajeHerramientaTextoBuilder>();
            mockBuilder.Setup(x => x.BuildAsync(It.IsAny<MensajeHerramienta>()))
                .ReturnsAsync("Tool response");

            Func<MensajeHerramienta, IMensajeHerramientaTextoBuilder> factory = 
                _ => mockBuilder.Object;

            return new ChatHistoryAdapter(factory);
        }

        [Test]
        public async Task AdaptAsync_MensajesOrdenados()
        {
            // Arrange
            List<Mensaje> mensajes =
            [new MensajeTextoUsuario() { Texto = "0", DateTime = DateTime.Now },
             new MensajeIA() { Texto = "1", DateTime = DateTime.Now }];

            var chatHistoryFactory = CreateChatHistoryAdapter();

            // Act
            var chatHistory = await chatHistoryFactory.AdaptAsync(mensajes);

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(
                        chatHistory[0].Role,
                        Is.EqualTo(AuthorRole.User));
                    Assert.That(chatHistory[0].ToString(), Is.EqualTo("0"));
                    Assert.That(
                        chatHistory[1].Role,
                        Is.EqualTo(AuthorRole.Assistant));
                    Assert.That(chatHistory[1].ToString(), Is.EqualTo("1"));
                });
        }

        [Test]
        public async Task AdaptAsync_MensajesDesordanados()
        {
            // Arrange
            List<Mensaje> mensajes =
            [new MensajeIA() { Texto = "1", DateTime = DateTime.Now },
             new MensajeTextoUsuario()
            {
                Texto = "0",
                DateTime = DateTime.Now.AddHours(-1)
            }];

            var chatHistoryFactory = CreateChatHistoryAdapter();

            // Act
            var chatHistory = await chatHistoryFactory.AdaptAsync(mensajes);

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(
                        chatHistory[0].Role,
                        Is.EqualTo(AuthorRole.User));
                    Assert.That(chatHistory[0].ToString(), Is.EqualTo("0"));
                    Assert.That(
                        chatHistory[1].Role,
                        Is.EqualTo(AuthorRole.Assistant));
                    Assert.That(chatHistory[1].ToString(), Is.EqualTo("1"));
                });
        }

        [Test]
        public async Task AdaptAsync_MensajeLlamadaHerramienta()
        {
            // Arrange
            var arguments = new Dictionary<string, object?>
            {
                ["param1"] = "value1",
                ["param2"] = 42
            };

            List<Mensaje> mensajes =
            [new MensajeLlamadaHerramienta()
            {
                FunctionName = "TestFunction",
                PluginName = "TestPlugin",
                Argumentos = arguments,
                DateTime = DateTime.Now
            }];

            var chatHistoryFactory = CreateChatHistoryAdapter();

            // Act
            var chatHistory = await chatHistoryFactory.AdaptAsync(mensajes);

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(chatHistory, Has.Count.EqualTo(1));
                    Assert.That(
                        chatHistory[0].Role,
                        Is.EqualTo(AuthorRole.Assistant));
                    Assert.That(chatHistory[0].Items, Has.Count.EqualTo(1));

                    var functionCall = chatHistory[0].Items.First() as FunctionCallContent;
                    Assert.That(functionCall, Is.Not.Null);
                    Assert.That(
                        functionCall?.FunctionName,
                        Is.EqualTo("TestFunction"));
                    Assert.That(
                        functionCall?.PluginName,
                        Is.EqualTo("TestPlugin"));
                });
        }

        [Test]
        public async Task Adapt_MensajeHerramientaInfo()
        {
            // Arrange
            var llamadaHerramienta = new MensajeLlamadaHerramienta()
            {
                FunctionName = "InfoFunction",
                PluginName = "InfoPlugin",
                Argumentos = new Dictionary<string, object?>(),
                DateTime = DateTime.Now.AddMinutes(-1)
            };

            var documentosRecuperados = new List<DocumentoRecuperado>
            {
                new() { DocumentoId = Guid.NewGuid(), Rank = true }
            };

            var consultasRecuperadas = new List<ConsultaRecuperada>
            {
                new() { ConsultaId = Guid.NewGuid(), Rank = true }
            };

            var mensajeHerramientaInfo = new MensajeHerramientaInfo(
                documentosRecuperados,
                consultasRecuperadas)
            {
                Llamada = llamadaHerramienta,
                DateTime = DateTime.Now
            };

            List<Mensaje> mensajes = [mensajeHerramientaInfo];

            var chatHistoryFactory = CreateChatHistoryAdapter();

            // Act
            var chatHistory = await chatHistoryFactory.AdaptAsync(mensajes);

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(chatHistory, Has.Count.EqualTo(1));
                    Assert.That(
                        chatHistory[0].Role,
                        Is.EqualTo(AuthorRole.Tool));
                    Assert.That(chatHistory[0].Items, Has.Count.EqualTo(1));

                    var functionResult = chatHistory[0].Items.First() as FunctionResultContent;
                    Assert.That(functionResult, Is.Not.Null);
                    Assert.That(
                        functionResult?.FunctionName,
                        Is.EqualTo("InfoFunction"));
                    Assert.That(
                        functionResult?.PluginName,
                        Is.EqualTo("InfoPlugin"));
                    Assert.That(
                        functionResult?.Result,
                        Is.EqualTo("Tool response"));
                });
        }

        [Test]
        public async Task Adapt_MensajesMixtos()
        {
            // Arrange
            var baseTime = DateTime.Now;

            List<Mensaje> mensajes =
            [
                new MensajeTextoUsuario()
            {
                Texto = "User message",
                DateTime = baseTime
            },
                new MensajeLlamadaHerramienta()
            {
                FunctionName = "GetInfo",
                PluginName = "InfoPlugin",
                Argumentos =
                    new Dictionary<string, object?> { ["query"] = "test" },
                DateTime = baseTime.AddMinutes(1)
            },
                new MensajeHerramientaInfo([], [])
            {
                Llamada =
                    new MensajeLlamadaHerramienta()
                    {
                        FunctionName = "GetInfo",
                        PluginName = "InfoPlugin",
                        Argumentos =
                            new Dictionary<string, object?>
                                {
                                    ["query"] = "test"
                                },
                        DateTime = baseTime.AddMinutes(1)
                    },
                DateTime = baseTime.AddMinutes(2)
            },
                new MensajeIA()
            {
                Texto = "AI response",
                DateTime = baseTime.AddMinutes(3)
            }];

            var chatHistoryFactory = CreateChatHistoryAdapter();

            // Act
            var chatHistory = await chatHistoryFactory.AdaptAsync(mensajes);

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(chatHistory, Has.Count.EqualTo(4));

                    // User message
                    Assert.That(
                        chatHistory[0].Role,
                        Is.EqualTo(AuthorRole.User));
                    Assert.That(
                        chatHistory[0].ToString(),
                        Is.EqualTo("User message"));

                    // Function call
                    Assert.That(
                        chatHistory[1].Role,
                        Is.EqualTo(AuthorRole.Assistant));
                    Assert.That(
                        chatHistory[1].Items.First(),
                        Is.TypeOf<FunctionCallContent>());

                    // Tool response
                    Assert.That(
                        chatHistory[2].Role,
                        Is.EqualTo(AuthorRole.Tool));
                    Assert.That(
                        chatHistory[2].Items.First(),
                        Is.TypeOf<FunctionResultContent>());

                    // AI response
                    Assert.That(
                        chatHistory[3].Role,
                        Is.EqualTo(AuthorRole.Assistant));
                    Assert.That(
                        chatHistory[3].ToString(),
                        Is.EqualTo("AI response"));
                });
        }

        [Test]
        public void Adapt_TipoMensajeNoSoportado_ThrowsNotSupportedException()
        {
            // Arrange
            var mensajeNoSoportado = new Mock<Mensaje>();

            List<Mensaje> mensajes = [mensajeNoSoportado.Object];

            var chatHistoryFactory = CreateChatHistoryAdapter();

            // Act & Assert
            var exception = Assert.ThrowsAsync<NotSupportedException>(
                async () => await chatHistoryFactory.AdaptAsync(mensajes));

            Assert.That(
                exception.Message,
                Does.Contain("Tipo de mensaje no soportado"));
        }
    }
}
