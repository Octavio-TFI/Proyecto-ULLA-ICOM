﻿using Domain.Entities.ChatAgregado;
using Domain.ValueObjects;
using Infrastructure.LLM.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
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

            var chatCompletionMock = new Mock<IChatCompletionService>();
            var chatHistoryFactoryMock = new Mock<IChatHistoryAdapter>();

            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton(chatCompletionMock.Object);

            var kernel = kernelBuilder.Build();

            var agent = new ChatCompletionAgent()
            {
                Kernel = kernel,
                Instructions = "{{$argument}}"
            };

            var agentData = new AgentData();

            chatHistoryFactoryMock
                .Setup(x => x.Adapt(mensajes))
                .Returns(chatHistory);

            chatCompletionMock
                .Setup(
                    x => x.GetChatMessageContentsAsync(
                        It.Is<ChatHistory>(c => c.First().Content == "system"),
                        It.IsAny<PromptExecutionSettings>(),
                        kernel,
                        default))
                .ReturnsAsync(
                    new List<ChatMessageContent>
                    {
                        new(AuthorRole.Assistant, expectedResponse)
                    }.AsReadOnly());

            var generadorRespuesta = new Agent(
                agent,
                agentData,
                chatHistoryFactoryMock.Object);

            // Act
            var result = await generadorRespuesta
                .GenerarRespuestaAsync(mensajes, arguments)
                .ConfigureAwait(false);

            // Assert
            Assert.That(result.Texto, Is.EqualTo(expectedResponse));
            Assert.That(result.AgentData, Is.EqualTo(agentData));
        }

        [Test]
        public async Task GenerarRespuesta_MensajeUnicoAsync()
        {
            // Arrange
            string mensaje = "Hola";
            string expectedResponse = "AI response";

            var chatCompletionMock = new Mock<IChatCompletionService>();
            var chatHistoryFactoryMock = new Mock<IChatHistoryAdapter>();

            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton(chatCompletionMock.Object);

            var kernel = kernelBuilder.Build();

            var agent = new ChatCompletionAgent() { Kernel = kernel };
            var agentData = new AgentData();

            chatCompletionMock
                .Setup(
                    x => x.GetChatMessageContentsAsync(
                        It.Is<ChatHistory>(c => c.Any(m => m.ToString() == mensaje)),
                        It.IsAny<PromptExecutionSettings>(),
                        kernel,
                        default))
                .ReturnsAsync(
                    new List<ChatMessageContent>
                    {
                        new(AuthorRole.Assistant, expectedResponse)
                    }.AsReadOnly());

            var generadorRespuesta = new Agent(
                agent,
                agentData,
                chatHistoryFactoryMock.Object);

            // Act
            var result = await generadorRespuesta
                .GenerarRespuestaAsync(mensaje)
                .ConfigureAwait(false);

            // Assert
            Assert.That(result.Texto, Is.EqualTo(expectedResponse));
            Assert.That(result.AgentData, Is.EqualTo(agentData));
        }
    }
}
