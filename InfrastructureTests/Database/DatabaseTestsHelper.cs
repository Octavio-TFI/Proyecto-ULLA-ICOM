using Infrastructure.Database.Chats;
using Infrastructure.Database.Embeddings;
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
            var options = new DbContextOptionsBuilder<EmbeddingContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new EmbeddingContext(options);
        }
    }
}
