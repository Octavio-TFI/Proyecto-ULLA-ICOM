using AppServices.Ports;
using Domain.Abstractions;
using Infrastructure.LLM.Abstractions;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

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

        [Test]
        public void AddToolTest()
        {
            // Arrange
            var agentBuilder = new AgentBuilder(
                new Mock<IServiceProvider>().Object,
                new Mock<IFileManager>().Object,
                new Mock<IChatHistoryAdapter>().Object);

            // Act
            agentBuilder.AddTool<TestTool>("test");

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(
                        agentBuilder.FunctionChoiceBehavior,
                        Is.TypeOf<AutoFunctionChoiceBehavior>());
                    Assert.That(agentBuilder.Tools, Has.Count.EqualTo(1));
                });

            var tool = agentBuilder.Tools[0];

            Assert.Multiple(
                () =>
                {
                    Assert.That(tool.Name, Is.EqualTo("test"));
                    Assert.That(tool.Count(), Is.EqualTo(2));
                });

            var function1 = tool.First();
            var function2 = tool.Last();

            Assert.Multiple(
                () =>
                {
                    Assert.That(function1.Name, Is.EqualTo("1"));
                    Assert.That(
                        function1.Description,
                        Is.EqualTo("Function Description 1"));
                    Assert.That(
                        function1.Metadata.ReturnParameter.ParameterType,
                        Is.EqualTo(typeof(string)));
                });

            var paremeter1 = function1.Metadata.Parameters.First();
            var paremeter2 = function2.Metadata.Parameters.First();

            Assert.Multiple(
                () =>
                {
                    Assert.That(paremeter1.Name, Is.EqualTo("testParameter"));
                    Assert.That(
                        paremeter1.Description,
                        Is.EqualTo("Parameter Description 1"));
                    Assert.That(paremeter2.Name, Is.EqualTo("testParameter2"));
                    Assert.That(
                        paremeter2.Description,
                        Is.EqualTo("Parameter Description 2"));
                });
        }

        private class TestTool(AgentData agentData)
        {
            [DisplayName("1")]
            [Description("Function Description 1")]
            public string TestFunction(
                [Description("Parameter Description 1")] string testParameter)
            {
                agentData.MetaData["test"] = testParameter;
                return "test";
            }

            [DisplayName("2")]
            [System.ComponentModel.Description("Function Description 2")]
            public string TestFunction2(
                [Description("Parameter Description 2")] string testParameter2)
            {
                agentData.MetaData["test2"] = testParameter2;
                return "test2";
            }
        }
    }
}
