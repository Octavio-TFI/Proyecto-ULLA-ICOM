using AppServices.Ports;
using Infrastructure.LLM.Abstractions;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.Tests
{
    class AgenBuilderTests
    {
        [Test]
        [TestCase(TipoLLM.Indefinido)]
        [TestCase(TipoLLM.Pequeño)]
        [TestCase(TipoLLM.Grande)]
        public void BuildTest(TipoLLM tipoLLM)
        {
            // Arrange
            PromptExecutionSettings executionSettings = new()
            {
                ServiceId = "serviceId"
            };

            var executionSettingsFactory = new Mock<IExecutionSettingsFactory>();
            executionSettingsFactory.Setup(x => x.Create(null, null, null))
                .Returns(executionSettings);

            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton(executionSettingsFactory.Object);
            var kernel = kernelBuilder.Build();

            var serviceProvider = new ServiceCollection()
                .AddKeyedSingleton(tipoLLM, kernel)
                .BuildServiceProvider();

            var fileManager = new Mock<IFileManager>();
            var chatHistoryFactory = new Mock<IChatHistoryAdapter>();

            var agentBuilder = new AgentBuilder(
                serviceProvider,
                fileManager.Object,
                chatHistoryFactory.Object);

            // Act
            var agent = agentBuilder
                .Build("name", tipoLLM);

            // Assert
            Assert.That(agent, Is.TypeOf<Agent>());

            var chatCompletionAgent = ((Agent)agent).ChatCompletionAgent;

            Assert.Multiple(
                () =>
                {
                    Assert.That(chatCompletionAgent.Name, Is.EqualTo("name"));
                    Assert.That(chatCompletionAgent.Instructions, Is.Null);
                    Assert.That(chatCompletionAgent.Kernel, Is.EqualTo(kernel));
                    Assert.That(
                        chatCompletionAgent.Arguments?.ExecutionSettings?[
                                "serviceId"],
                        Is.EqualTo(executionSettings));
                });
        }

        [Test]
        public void SetResponseSchemaTest()
        {
            // Arrange
            var agentBuilder = new AgentBuilder(
                new Mock<IServiceProvider>().Object,
                new Mock<IFileManager>().Object,
                new Mock<IChatHistoryAdapter>().Object);

            // Act
            agentBuilder.SetResponseSchema(typeof(string));

            // Assert
            Assert.That(agentBuilder.Schema, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void SetTemperatureTest()
        {
            // Arrange
            var agentBuilder = new AgentBuilder(
                new Mock<IServiceProvider>().Object,
                new Mock<IFileManager>().Object,
                new Mock<IChatHistoryAdapter>().Object);

            // Act
            agentBuilder.SetTemperature(0.5);

            // Assert
            Assert.That(agentBuilder.Temperature, Is.EqualTo(0.5));
        }
    }
}
