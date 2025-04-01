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
                    128,
                    "application/json");
                yield return new TestCaseData(
                    FunctionChoiceBehavior.Required(),
                    typeof(int),
                    0.5,
                    0,
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
            FunctionChoiceBehavior? functionChoiceBehavior,
            Type? schema,
            double? temperature,
            int? expectedGeminiBehaviorAutoInvokeCount,
            string? expectedMimeType)
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
                        executionSettings.ToolCallBehavior?.MaximumAutoInvokeAttempts,
                        Is.EqualTo(expectedGeminiBehaviorAutoInvokeCount));
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
