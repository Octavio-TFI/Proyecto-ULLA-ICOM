using Domain.Abstractions;
using Domain.Entities.DocumentoAgregado;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Tests
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
            new() { Filename = "fileName", Texto = "2" }];

            var consulta = "consulta";

            var agentMock = new Mock<IAgent>();

            agentMock.Setup(
                x => x.GenerarRespuestaAsync(
                    consulta,
                    It.Is<Dictionary<string, object?>>(
                        a => a.First().Key == "document" &&
                            a.First().Value == datosRecuperados[0])))
                .ReturnsAsync(
                    new AgentResult
                    {
                        Texto = TrueResult,
                        AgentData = new()
                    });

            agentMock.Setup(
                x => x.GenerarRespuestaAsync(
                    consulta,
                    It.Is<Dictionary<string, object?>>(
                        a => a.First().Key == "document" &&
                            a.First().Value == datosRecuperados[1])))
                .ReturnsAsync(
                    new AgentResult
                    {
                        Texto = FalseResult,
                        AgentData = new()
                    });

            agentMock.Setup(
                x => x.GenerarRespuestaAsync(
                    consulta,
                    It.Is<Dictionary<string, object?>>(
                        a => a.First().Key == "document" &&
                            a.First().Value == datosRecuperados[2])))
                .ReturnsAsync(
                    new AgentResult
                    {
                        Texto = string.Empty,
                        AgentData = new()
                    });

            var ranker = new Ranker(agentMock.Object);

            // Act
            var result = await ranker.RankAsync(datosRecuperados, consulta)
                .ConfigureAwait(false);

            // Assert
            agentMock.Verify(
                x => x.GenerarRespuestaAsync(
                    consulta,
                    It.IsAny<Dictionary<string, object?>>()),
                Times.Exactly(3));

            Assert.That(result, Has.Member(datosRecuperados[0]));
            Assert.That(result, Has.No.Member(datosRecuperados[1]));
            Assert.That(result, Has.No.Member(datosRecuperados[2]));
        }
    }
}
