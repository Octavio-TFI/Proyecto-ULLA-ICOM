using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppServices.ConsultasProcessing.Tests
{
    class ConsultaProcessorTests
    {
        [Test]
        public async Task ProcessAsync_Success()
        {
            // Arrange
            var consultaData = new ConsultaData
            {
                Id = 1,
                Titulo = "Test Title",
                Descripcion = "Test Description",
                PreFixes = new string[] { "Prefix1", "Prefix2" },
                Fix = "Test Fix"
            };

            var chatHistory = new ChatHistory();
            var agentResult = new ChatMessageContent(
                AuthorRole.Assistant,
                JsonConvert.SerializeObject(
                    new ConsultaResumen
                {
                    Descripcion = "Processed Description",
                    Solucion = "Processed Solution"
                }));

            var mockChatCompletionService = new Mock<IChatCompletionService>();
            var mockEmbeddingService = new Mock<ITextEmbeddingGenerationService>(
                );

            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services
                .AddSingleton(mockChatCompletionService.Object);
            var kernel = kernelBuilder.Build();

            var agent = new ChatCompletionAgent { Kernel = kernel };
            var consultaProcessor = new ConsultaProcessor(
                agent,
                mockEmbeddingService.Object);

            mockChatCompletionService.Setup(
                x => x.GetChatMessageContentsAsync(
                    It.Is<ChatHistory>(
                        x => x.First().Content == consultaData.ToString()),
                    It.IsAny<PromptExecutionSettings>(),
                    agent.Kernel,
                    default))
                .ReturnsAsync(
                    new List<ChatMessageContent> { agentResult }.AsReadOnly());

            mockEmbeddingService.Setup(
                e => e.GenerateEmbeddingsAsync(
                    It.Is<IList<string>>(
                        x => x.First() == consultaData.Titulo &&
                            x.Last() == "Processed Description"),
                    null,
                    default))
                .ReturnsAsync(
                    [new float[] { 0.1f, 0.2f },new float[] { 0.3f, 0.4f }]);

            // Act
            var result = await consultaProcessor.ProcessAsync(consultaData);

            // Assert
            Assert.That(result.Id, Is.EqualTo(consultaData.Id));
            Assert.That(result.Titulo, Is.EqualTo(consultaData.Titulo));
            Assert.That(result.Descripcion, Is.EqualTo("Processed Description"));
            Assert.That(result.Solucion, Is.EqualTo("Processed Solution"));
            Assert.That(
                result.EmbeddingTitulo,
                Is.EqualTo(new float[] { 0.1f, 0.2f }));
            Assert.That(
                result.EmbeddingDescripcion,
                Is.EqualTo(new float[] { 0.3f, 0.4f }));
        }

        [Test]
        public void ProcessAsync_DeserializationError_ThrowsException()
        {
            // Arrange
            var consultaData = new ConsultaData
            {
                Id = 1,
                Titulo = "Test Title",
                Descripcion = "Test Description",
                PreFixes = ["Prefix1", "Prefix2"],
                Fix = "Test Fix"
            };

            var chatHistory = new ChatHistory();
            var agentResult = new ChatMessageContent(
                AuthorRole.Assistant,
                string.Empty);

            var mockChatCompletionService = new Mock<IChatCompletionService>();
            var mockEmbeddingService = new Mock<ITextEmbeddingGenerationService>(
                );

            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services
                .AddSingleton(mockChatCompletionService.Object);
            var kernel = kernelBuilder.Build();

            var agent = new ChatCompletionAgent { Kernel = kernel };
            var consultaProcessor = new ConsultaProcessor(
                agent,
                mockEmbeddingService.Object);

            mockChatCompletionService.Setup(
                x => x.GetChatMessageContentsAsync(
                    It.IsAny<ChatHistory>(),
                    It.IsAny<PromptExecutionSettings>(),
                    agent.Kernel,
                    default))
                .ReturnsAsync(
                    new List<ChatMessageContent> { agentResult }.AsReadOnly());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(
                async () => await consultaProcessor.ProcessAsync(consultaData));
        }
    }
}
