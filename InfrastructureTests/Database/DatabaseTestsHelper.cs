using Microsoft.Data.Sqlite;
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
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            connection.LoadExtension("vec0");

            var options = new DbContextOptionsBuilder<ChatContext>()
                .UseSqlite(connection)
                .Options;

            var context = new ChatContext(options);

            context.Database.EnsureCreated();

            return context;
        }
    }
}
