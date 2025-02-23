using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.ExecutionSettingsFactories.Tests
{
    class OpenAiExecutionSettingsFactoryTests
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
            double temperature)
        {
            // Arrange
            var openAiExecutionSettingsFactory = new OpenAiExecutionSettingsFactory(
                );

            // Act
            var executionSettings = openAiExecutionSettingsFactory.Create(
                functionChoiceBehavior,
                schema,
                temperature) as OpenAIPromptExecutionSettings;

            // Assert
            Assert.That(executionSettings, Is.Not.Null);
            Assert.Multiple(
                () =>
                {
                    Assert.That(
                        executionSettings.FunctionChoiceBehavior,
                        Is.EqualTo(functionChoiceBehavior));
                    Assert.That(
                        executionSettings.ResponseFormat,
                        Is.EqualTo(schema));
                    Assert.That(
                        executionSettings.Temperature,
                        Is.EqualTo(temperature));
                });
        }
    }
}
