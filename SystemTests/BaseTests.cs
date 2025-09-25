using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace System.Tests
{
    public abstract class BaseTests
    {
        readonly Dictionary<string, APIFactory> _apiFactories = [];

        protected async Task<APIFactory> CreateAPIFactoryAsync(
            int LLMPort,
            int testClientPort)
        {
            var sqlContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2025-latest")
                .Build();

            await sqlContainer.StartAsync();

            var apiFactory = new APIFactory(
                LLMPort,
                testClientPort,
                sqlContainer.GetConnectionString());

            string testId = Guid.NewGuid().ToString();

            _apiFactories.Add(testId, apiFactory);

            return apiFactory;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var apiFactory in _apiFactories.Values)
            {
                apiFactory.Dispose();
            }
        }
    }
}
