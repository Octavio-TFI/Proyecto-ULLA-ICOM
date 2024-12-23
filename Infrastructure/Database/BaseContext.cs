using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    internal abstract class BaseContext : DbContext
    {
        public DbSet<OutboxEvent> OutboxEvents { get; set; }

        public BaseContext() : base()
        {
        }

        public BaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureOutboxEventModel(modelBuilder);
        }

        static void ConfigureOutboxEventModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutboxEvent>().HasKey(e => e.Id);
        }
    }
}
