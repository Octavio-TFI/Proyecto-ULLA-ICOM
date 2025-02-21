using AppServices.Ranking;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Ranking.Tests
{
    internal class RankerTests
    {
        static readonly string TrueResult = JsonConvert.SerializeObject(
            new RankerResult() { Score = true });

        static readonly string FalseResult = JsonConvert.SerializeObject(
            new RankerResult() { Score = false });

        [Test]
        public async Task RankAsyncTask()
        {
            // Arrange
            List<Document> datosRecuperados = [
            new() { Filename = "fileName", Texto = "0" },
            new() { Filename = "fileName", Texto = "1" },
            new() { Filename = "fileName", Texto = "2" },
            new() { Filename = "fileName", Texto = "3" }];

            var consulta = "consulta";

            var chatCompletionMock = new Mock<IChatCompletionService>();

            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton(chatCompletionMock.Object);
            var kernel = kernelBuilder.Build();

            chatCompletionMock.Setup(
                x => x.GetChatMessageContentsAsync(
                    It.Is<ChatHistory>(
                        c => c.First().Role == AuthorRole.System &&
                            c.First().Content == "0"),
                    It.IsAny<PromptExecutionSettings>(),
                    kernel,
                    default))
                .ReturnsAsync(
                    new List<ChatMessageContent>
                    {
                        new(AuthorRole.Assistant, TrueResult)
                    }.AsReadOnly());

            chatCompletionMock.Setup(
                x => x.GetChatMessageContentsAsync(
                    It.Is<ChatHistory>(
                        c => c.First().Role == AuthorRole.System &&
                            c.First().Content == "1"),
                    It.IsAny<PromptExecutionSettings>(),
                    kernel,
                    default))
                .ReturnsAsync(
                    new List<ChatMessageContent>
                    {
                        new(AuthorRole.Assistant, FalseResult)
                    }.AsReadOnly());

            chatCompletionMock.Setup(
                x => x.GetChatMessageContentsAsync(
                    It.Is<ChatHistory>(
                        c => c.First().Role == AuthorRole.System &&
                            c.First().Content == "2"),
                    It.IsAny<PromptExecutionSettings>(),
                    kernel,
                    default))
                .ReturnsAsync(
                    new List<ChatMessageContent>
                    {
                        new(AuthorRole.Assistant, string.Empty)
                    }.AsReadOnly());

            chatCompletionMock.Setup(
                x => x.GetChatMessageContentsAsync(
                    It.Is<ChatHistory>(
                        c => c.First().Role == AuthorRole.System &&
                            c.First().Content == "3"),
                    It.IsAny<PromptExecutionSettings>(),
                    kernel,
                    default))
                .ReturnsAsync(
                    new List<ChatMessageContent>
                    {
                        new(AuthorRole.Assistant, (string?)null)
                    }.AsReadOnly());

            var agent = new ChatCompletionAgent()
            {
                Name = "nombre",
                Instructions = "{{$document}}",
                Kernel = kernel,
                LoggerFactory =
                    LoggerFactory.Create(builder => builder.AddConsole())
            };

            var ranker = new Ranker(agent);

            // Act
            var result = await ranker.RankAsync(datosRecuperados, consulta);

            // Assert
            chatCompletionMock.Verify(
                x => x.GetChatMessageContentsAsync(
                    It.Is<ChatHistory>(
                        c => c.Skip(1).First().Role == AuthorRole.User &&
                            c.Skip(1).First().Content == consulta),
                    It.IsAny<PromptExecutionSettings>(),
                    kernel,
                    default),
                Times.Exactly(4));

            Assert.That(result, Has.Member(datosRecuperados[0]));
            Assert.That(result, Has.No.Member(datosRecuperados[1]));
            Assert.That(result, Has.No.Member(datosRecuperados[2]));
            Assert.That(result, Has.No.Member(datosRecuperados[3]));
        }
    }
}
