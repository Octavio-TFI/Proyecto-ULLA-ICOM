﻿using Domain.Entities;
using Infrastructure.Database.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Embeddings
{
    internal class EmbeddingContext
        : BaseContext
    {
        public DbSet<Document> Documents { get; set; }

        public DbSet<DocumentChunk> DocumentChunks { get; set; }

        public DbSet<Consulta> Consultas { get; set; }

        public EmbeddingContext() : base()
        {
        }

        public EmbeddingContext(DbContextOptions<EmbeddingContext> options) : base(
            options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // modelBuilder.HasDefaultSchema("Embedding");
            modelBuilder.HasDbFunction(
                typeof(EmbeddingContext).GetMethod(nameof(CosineDistance))!)
                .HasName("vec_distance_cosine");

            ConfigureDocumentModel(modelBuilder);
            ConfigureConsultaModel(modelBuilder);
        }

        static void ConfigureDocumentModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>().HasKey(e => e.Id);

            modelBuilder.Entity<Document>().HasIndex(e => e.Filename).IsUnique();

            modelBuilder.Entity<Document>()
                .HasMany(c => c.Chunks)
                .WithOne()
                .IsRequired();
        }

        static void ConfigureDocumentChunkModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentChunk>().HasKey(e => e.Id);
            modelBuilder.Entity<DocumentChunk>()
                .Property(e => e.Embedding)
                .HasColumnType("float[768]");
        }

        static void ConfigureConsultaModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Consulta>().HasKey(e => e.Id);

            modelBuilder.Entity<Consulta>()
                .Property(e => e.EmbeddingTitulo)
                .HasColumnType("float[768]");

            modelBuilder.Entity<Consulta>()
                .Property(e => e.EmbeddingDescripcion)
                .HasColumnType("float[768]");
        }

        public double CosineDistance(float[] vector1, float[] vector2)
        {
            throw new NotSupportedException();
        }
    }
}
