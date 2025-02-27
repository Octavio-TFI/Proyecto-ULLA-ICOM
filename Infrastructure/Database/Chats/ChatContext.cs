using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Chats
{
    internal class ChatContext : BaseContext
    {
        public DbSet<Chat> Chats { get; set; }

        public DbSet<Mensaje> Mensajes { get; set; }

        public DbSet<MensajeTexto> MensajesDeTexto { get; set; }

        public ChatContext() : base()
        {
        }

        public ChatContext(DbContextOptions<ChatContext> options) : base(
            options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("Chat");

            ConfigureChatModel(modelBuilder);
            ConfigureMensajeModel(modelBuilder);
        }

        static void ConfigureChatModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>().HasKey(c => c.Id);

            modelBuilder.Entity<Chat>()
                .HasIndex(
                    c => new { c.UsuarioId, c.ChatPlataformaId, c.Plataforma })
                .IsUnique();
        }

        static void ConfigureMensajeModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Mensaje>().HasKey(m => m.Id);

            modelBuilder.Entity<Mensaje>()
                .HasOne<Chat>()
                .WithMany(c => c.Mensajes)
                .IsRequired()
                .HasForeignKey(x => x.ChatId);

            modelBuilder.Entity<Mensaje>().UseTpcMappingStrategy();
        }
    }
}
