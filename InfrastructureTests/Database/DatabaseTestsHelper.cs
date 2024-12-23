using Infrastructure.Database;
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
        public static ChatContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ChatContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ChatContext(options);
        }
    }
}
