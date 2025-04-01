using Infrastructure.LLM.DTOs;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.Tests
{
    internal class LMStudioEmbeddingServiceTests
    {
        [Test]
        public void GenerateAsync_UnsuccessfulStatusCode_ShouldThrow()
        {
            // Arrange
            var data = new List<string> { "data" };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            var httpClient = MockHttpClient(response);

            var service = new LMStudioEmbeddingService(httpClient);

            // Act
            async Task Act()
                => await service.GenerateAsync(data).ConfigureAwait(false);

            // Assert
            Assert.ThrowsAsync<Exception>(Act);
        }

        [Test]
        public void GenerateAsync_NullResponse_ShouldThrow()
        {
            // Arrange
            var data = new List<string> { "data" };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create<EmbeddingResponseList>(null!)
            };

            var httpClient = MockHttpClient(response);

            var service = new LMStudioEmbeddingService(httpClient);

            // Act
            async Task Act()
                => await service.GenerateAsync(data).ConfigureAwait(false);

            // Assert
            Assert.ThrowsAsync<Exception>(Act);
        }

        [Test]
        public async Task GenerateAsync_SuccessfulResponse_ShouldReturnEmbeddings(
            )
        {
            // Arrange
            var data = new List<string> { "data" };
            var embeddingResponse = new EmbeddingResponseList
            {
                Data =
                    [new() { Embedding = [1,2,3] },
                    new() { Embedding = [4,5,6] }]
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(embeddingResponse)
            };

            var httpClient = MockHttpClient(response);

            var service = new LMStudioEmbeddingService(httpClient);

            // Act
            var result = await service.GenerateAsync(data).ConfigureAwait(false);

            // Assert
            Assert.That(
                result.Select(x => x.ToArray()).ToList(),
                Is.EqualTo(
                    embeddingResponse.Data.Select(x => x.Embedding).ToList()));
        }

        [Test]
        public async Task GenerateAsyncSingleString_SuccessfulResponse_ShouldReturnEmbeddings(
            )
        {
            // Arrange
            var embeddingResponse = new EmbeddingResponseList
            {
                Data = [new() { Embedding = [1, 2, 3] }]
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(embeddingResponse)
            };

            var httpClient = MockHttpClient(response);

            var service = new LMStudioEmbeddingService(httpClient);

            // Act
            var result = await service.GenerateAsync("data")
                .ConfigureAwait(false);

            // Assert
            Assert.That(
                result,
                Is.EqualTo(
                    embeddingResponse.Data.Select(x => x.Embedding).First()));
        }

        static HttpClient MockHttpClient(HttpResponseMessage response)
        {
            var httpClient = new HttpClient(MockHttpMessageHandler(response))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            return httpClient;
        }

        static HttpMessageHandler MockHttpMessageHandler(
            HttpResponseMessage response)
        {
            var mock = new Mock<HttpMessageHandler>();

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            return mock.Object;
        }
    }
}
