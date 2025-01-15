using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AppServices.Ranker;

namespace AppServices.Tests
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
            List<Document> datosRecuperados = [new()
            {
                Texto = "0",
                Embedding = [0]
            },
            new() { Texto = "1", Embedding = [1] },
            new() { Texto = "2", Embedding = [2] },
            new() { Texto = "3", Embedding = [3] }];

            var consulta = "consulta";
            int i = -1;

            var kernelFunction = KernelFunctionFactory.CreateFromMethod(
                () =>
                {
                    i++;

                    return i switch
                    {
                        0 => TrueResult,
                        1 => FalseResult,
                        3 => string.Empty,
                        _ => null
                    };
                },
                functionName: "Document");

            var kernel = new Kernel();
            kernel.ImportPluginFromFunctions("Ranker", [kernelFunction]);

            var ranker = new Ranker(kernel);

            // Act
            var result = await ranker.RankAsync(datosRecuperados, consulta);

            // Assert
            Assert.That(result, Has.Member(datosRecuperados[0]));
            Assert.That(result, Has.No.Member(datosRecuperados[1]));
            Assert.That(result, Has.No.Member(datosRecuperados[2]));
            Assert.That(result, Has.No.Member(datosRecuperados[3]));
        }
    }
}
