using DotNet.Testcontainers.Builders;
using Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace System.Tests
{
    public class APIFactory(int LLMPort, int testClientPort, string connectionString)
        : WebApplicationFactory<APIProgram>
    {
        readonly string _connectionString = connectionString;

        readonly int _LLMPort = LLMPort;
        readonly int _testClientPort = testClientPort;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                { "LLMLocal:URL", $"http://localhost:{_LLMPort}" },
                { "LLMGoogle:URL", $"http://localhost:{_LLMPort}" },
                { "LLMGoogle:ApiKey", "ApiKey" },
                { "LLMGoogle:Model", "Model" },
                { "ConnectionStrings:Default", _connectionString },
                { "Clients:Test:URL", $"http://localhost:{_testClientPort}" }
                })
                .Build();

            var context = CreateContext();

            context.Database.EnsureCreated();

            builder.UseConfiguration(config);
        }

        ChatContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ChatContext>()
                .UseSqlServer(
                    _connectionString,
                    options => options.UseVectorSearch())
                .Options;

            return new ChatContext(options);
        }
    }
}
