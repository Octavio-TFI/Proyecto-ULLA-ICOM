using Infrastructure.Database.Chats;
using Infrastructure.Database.Embeddings;
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
            var options = new DbContextOptionsBuilder<ChatContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ChatContext(options);
        }

        public static EmbeddingContext CreateInMemoryEmbeddingContext()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            connection.LoadExtension("vec0");

            var options = new DbContextOptionsBuilder<EmbeddingContext>()
                .UseSqlite(connection)
                .Options;

            var context = new EmbeddingContext(options);

            context.Database.EnsureCreated();

            return context;
        }
    }
}
