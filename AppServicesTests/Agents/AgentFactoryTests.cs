using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;

namespace AppServices.Agents.Tests
{
    [TestFixture]
    public class AgentFactoryTests
    {
        static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(
                    FunctionChoiceBehavior.Auto(),
                    typeof(string),
                    0.7);
                yield return new TestCaseData(
                    FunctionChoiceBehavior.Required(),
                    typeof(int),
                    0.5);
                yield return new TestCaseData(
                    FunctionChoiceBehavior.None(),
                    null,
                    null);
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void CreateTest(
            FunctionChoiceBehavior functionChoiceBehavior,
            Type schema,
            double? temperature)
        {
            // Arrange
            var fileManagerMock = new Mock<IFileManager>();
            var kernelBuilder = Kernel.CreateBuilder();
            var executionSettingsFactoryMock = new Mock<IExecutionSettingsFactory>(
                );

            var executionSettings = new PromptExecutionSettings();

            executionSettingsFactoryMock.Setup(
                esf => esf.Create(functionChoiceBehavior, schema, temperature))
                .Returns(executionSettings);

            kernelBuilder.Services
                .AddSingleton(executionSettingsFactoryMock.Object);
            var kernel = kernelBuilder.Build();
            var agentFactory = new AgentFactory(fileManagerMock.Object);

            string name = "TestAgent";
            string instructions = "Test Instructions";
            fileManagerMock.Setup(fm => fm.ReadAllText(It.IsAny<string>()))
                .Returns(instructions);

            // Act
            var agent = agentFactory.Create(
                kernel,
                name,
                functionChoiceBehavior,
                schema,
                temperature);

            // Assert
            Assert.That(agent.Name, Is.EqualTo(name));
            Assert.That(agent.Instructions, Is.EqualTo(instructions));
            Assert.That(agent.Kernel, Is.EqualTo(kernel));
            Assert.That(
                agent.Arguments?.ExecutionSettings?.Last().Value,
                Is.EqualTo(executionSettings));
        }
    }
}
