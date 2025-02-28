﻿// <auto-generated />
using System;
using Infrastructure.Database.Embeddings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Database.Embeddings.Migrations
{
    [DbContext(typeof(EmbeddingContext))]
    partial class EmbeddingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("Domain.Entities.Consulta", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("EmbeddingDescripcion")
                        .IsRequired()
                        .HasColumnType("float[768]");

                    b.PrimitiveCollection<string>("EmbeddingTitulo")
                        .IsRequired()
                        .HasColumnType("float[768]");

                    b.Property<int>("RemoteId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Solucion")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Consultas");
                });

            modelBuilder.Entity("Domain.Entities.Document", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Filename")
                        .IsUnique();

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("Domain.Entities.DocumentChunk", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("DocumentId")
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("Embedding")
                        .IsRequired()
                        .HasColumnType("float[768]");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.ToTable("DocumentChunks");
                });

            modelBuilder.Entity("Infrastructure.Outbox.OutboxEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("EventData")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsProcessed")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("OccurredOn")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ProcessedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("OutboxEvents");
                });

            modelBuilder.Entity("Domain.Entities.DocumentChunk", b =>
                {
                    b.HasOne("Domain.Entities.Document", null)
                        .WithMany("Chunks")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.Document", b =>
                {
                    b.Navigation("Chunks");
                });
#pragma warning restore 612, 618
        }
    }
}
