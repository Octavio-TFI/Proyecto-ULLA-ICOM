using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.ExecutionSettingsFactories.Tests
{
    class GeminiExecutionSettingsFactoryTests
    {
        static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(
                    FunctionChoiceBehavior.Auto(),
                    typeof(string),
                    0.7,
                    GeminiToolCallBehavior.AutoInvokeKernelFunctions,
                    "application/json");
                yield return new TestCaseData(
                    FunctionChoiceBehavior.Required(),
                    typeof(int),
                    0.5,
                    GeminiToolCallBehavior.EnableKernelFunctions,
                    "application/json");
                yield return new TestCaseData(
                    FunctionChoiceBehavior.None(),
                    null,
                    null,
                    null,
                    null);
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void CreateTest(
            FunctionChoiceBehavior functionChoiceBehavior,
            Type schema,
            double temperature,
            GeminiToolCallBehavior expectedGeminiBehavior,
            string expectedMimeType)
        {
            // Arrange
            var openAiExecutionSettingsFactory = new GeminiExecutionSettingsFactory(
                );

            // Act
            var executionSettings = openAiExecutionSettingsFactory.Create(
                functionChoiceBehavior,
                schema,
                temperature) as GeminiPromptExecutionSettings;

            // Assert
            Assert.That(executionSettings, Is.Not.Null);
            Assert.Multiple(
                () =>
                {
                    Assert.That(
                        executionSettings.FunctionChoiceBehavior,
                        Is.EqualTo(functionChoiceBehavior));
                    Assert.That(
                        executionSettings.ToolCallBehavior,
                        Is.EqualTo(expectedGeminiBehavior));
                    Assert.That(
                        executionSettings.ResponseSchema,
                        Is.EqualTo(schema));
                    Assert.That(
                        executionSettings.ResponseMimeType,
                        Is.EqualTo(expectedMimeType));
                    Assert.That(
                        executionSettings.Temperature,
                        Is.EqualTo(temperature));
                });
        }
    }
}
