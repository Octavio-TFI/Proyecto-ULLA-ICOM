using Domain.Entities;
using Domain.Entities.ChatAgregado;
using Domain.Entities.ConsultaAgregado;
using Domain.Entities.DocumentoAgregado;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    internal class ChatContext : DbContext
    {
        public DbSet<OutboxEvent> OutboxEvents { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<Consulta> Consultas { get; set; }

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

            ConfigureChatModel(modelBuilder.Entity<Chat>());
            ConfigureMensajeModel(modelBuilder.Entity<Mensaje>());
            ConfigureDocumentoRecuperadoModel(
                modelBuilder.Entity<DocumentoRecuperado>());
            ConfigureConsultaRecuperadaModel(
                modelBuilder.Entity<ConsultaRecuperada>());

            modelBuilder.Entity<MensajeIA>().HasBaseType<Mensaje>();
            modelBuilder.Entity<MensajeTextoUsuario>().HasBaseType<Mensaje>();

            modelBuilder.HasDbFunction(
                typeof(ChatContext).GetMethod(nameof(CosineDistance))!)
                .HasName("vec_distance_cosine");

            ConfigureDocumentModel(modelBuilder.Entity<Document>());
            ConfigureDocumentChunkModel(modelBuilder.Entity<DocumentChunk>());
            ConfigureConsultaModel(modelBuilder.Entity<Consulta>());
        }

        static void ConfigureChatModel(EntityTypeBuilder<Chat> chatBuilder)
        {
            chatBuilder
                .HasIndex(
                    c => new { c.UsuarioId, c.ChatPlataformaId, c.Plataforma })
                .IsUnique();

            chatBuilder
                .HasMany(c => c.Mensajes)
                .WithOne()
                .IsRequired();
        }

        static void ConfigureMensajeModel(EntityTypeBuilder<Mensaje> mensajeBuilder)
        {
            mensajeBuilder.UseTpcMappingStrategy();
        }

        static void ConfigureDocumentoRecuperadoModel(
            EntityTypeBuilder<DocumentoRecuperado> documentoRecuperadoBuilder)
        {
            documentoRecuperadoBuilder
                .HasOne<MensajeIA>()
                .WithMany(m => m.DocumentosRecuperados)
                .IsRequired();

            documentoRecuperadoBuilder
                .HasOne<Document>()
                .WithMany()
                .IsRequired()
                .HasForeignKey(dr => dr.DocumentoId);
        }

        static void ConfigureConsultaRecuperadaModel(
            EntityTypeBuilder<ConsultaRecuperada> consultaRecuperadaBuilder)
        {
            consultaRecuperadaBuilder
                .HasOne<MensajeIA>()
                .WithMany(m => m.ConsultasRecuperadas)
                .IsRequired();

            consultaRecuperadaBuilder
                .HasOne<Consulta>()
                .WithMany()
                .IsRequired()
                .HasForeignKey(cr => cr.ConsultaId);
        }

        static void ConfigureDocumentModel(
            EntityTypeBuilder<Document> documentBuilder)
        {
            documentBuilder.HasIndex(e => e.Filename).IsUnique();

            documentBuilder
                .HasMany(c => c.Chunks)
                .WithOne()
                .IsRequired();
        }

        static void ConfigureDocumentChunkModel(
            EntityTypeBuilder<DocumentChunk> docmuentChunckBuilder)
        {
            docmuentChunckBuilder
                .Property(e => e.Embedding)
                .HasColumnType("float[768]");
        }

        static void ConfigureConsultaModel(
            EntityTypeBuilder<Consulta> consultaBuilder)
        {
            consultaBuilder
                .Property(e => e.EmbeddingTitulo)
                .HasColumnType("float[768]");

            consultaBuilder
                .Property(e => e.EmbeddingDescripcion)
                .HasColumnType("float[768]");
        }

        public double CosineDistance(float[] vector1, float[] vector2)
        {
            throw new NotSupportedException();
        }
    }
}
