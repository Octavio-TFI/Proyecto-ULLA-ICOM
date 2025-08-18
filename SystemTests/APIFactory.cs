using Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Tests
{
    public class APIFactory(int LLMPort, int testClientPort, string dbName)
        : WebApplicationFactory<APIProgram>
    {
        readonly string _connectionString = $"Data Source={dbName}.db";

        readonly int LLMPort = LLMPort;
        readonly int testClientPort = testClientPort;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                { "LLMLocal:URL", $"http://localhost:{LLMPort}" },
                { "LLMGoogle:URL", $"http://localhost:{LLMPort}" },
                { "LLMGoogle:ApiKey", "ApiKey" },
                { "LLMGoogle:Model", "Model" },
                { "ConnectionStrings:Default", _connectionString },
                { "Clients:Test:URL", $"http://localhost:{testClientPort}" }
                })
                .Build();

            var context = CreateContext();

            context.Database.EnsureCreated();

            builder.UseConfiguration(config);
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync().ConfigureAwait(false);

            var context = CreateContext();

            await context.Database.EnsureDeletedAsync().ConfigureAwait(false);
        }

        ChatContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ChatContext>()
                .UseSqlite(_connectionString)
                .Options;

            return new ChatContext(options);
        }
    }
}
