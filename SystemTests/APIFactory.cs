using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Tests
{
    internal class APIFactory(int localLLMPort, int nubeLLMPort)
        : WebApplicationFactory<APIProgram>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            var config = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                { "URLs:LocalLLM", $"http://localhost:{localLLMPort}" },
                { "URLs:NubeLLM", $"http://localhost:{nubeLLMPort}" }
                })
                .Build();

            builder.UseConfiguration(config);
        }
    }
}
