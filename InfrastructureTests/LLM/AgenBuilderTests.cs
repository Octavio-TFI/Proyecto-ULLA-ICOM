using AppServices.Ports;
using Infrastructure.LLM.Abstractions;
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
        public void BuildTest()
        {
            // Arrange
            var serviceProvider = new Mock<IServiceProvider>();
            var fileManager = new Mock<IFileManager>();
            var chatHistoryFactory = new Mock<IChatHistoryAdapter>();

            var agentBuilder = new AgentBuilder(
                serviceProvider.Object,
                fileManager.Object,
                chatHistoryFactory.Object);

            // Act
            var agent = agentBuilder
                .Build("name", TipoLLM.Pequeño);

            // Assert
            Assert.Fail();
        }
    }
}
