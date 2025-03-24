﻿using Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Tests
{
    public class APIFactory(int localLLMPort, int nubeLLMPort, string dbName)
        : WebApplicationFactory<APIProgram>
    {
        readonly string _connectionString = $"Data Source={dbName}.db";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                { "URLs:LLMLocal", $"http://localhost:{localLLMPort}" },
                { "URLs:LLMNube", $"http://localhost:{nubeLLMPort}" },
                { "ConnectionStrings:Default", _connectionString }
                })
                .Build();

            var context = CreateContext();

            context.Database.EnsureCreated();

            builder.UseConfiguration(config);
        }

        public override async ValueTask DisposeAsync()
        {
            var context = CreateContext();

            await context.Database.EnsureDeletedAsync();

            await base.DisposeAsync();
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
