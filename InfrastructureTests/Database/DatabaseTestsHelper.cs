using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureTests.Database.Tests
{
    internal static class DatabaseTestsHelper
    {
        public static ChatContext CreateInMemoryChatContext()
        {
            var options = new DbContextOptionsBuilder<ChatContext>()
                .UseInMemoryDatabase(TestContext.CurrentContext.Test.ID)
                .Options;

            var context = new ChatContext(options);

            context.Database.EnsureCreated();

            return context;
        }
    }
}
