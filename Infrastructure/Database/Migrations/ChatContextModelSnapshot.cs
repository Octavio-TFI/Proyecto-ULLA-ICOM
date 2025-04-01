﻿// <auto-generated />
using System;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    [DbContext(typeof(ChatContext))]
    partial class ChatContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("Domain.Entities.ChatAgregado.Chat", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ChatPlataformaId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Plataforma")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UsuarioId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioId", "ChatPlataformaId", "Plataforma")
                        .IsUnique();

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("Domain.Entities.ChatAgregado.ConsultaRecuperada", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ConsultaId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MensajeIAId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Rank")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ConsultaId");

                    b.HasIndex("MensajeIAId");

                    b.ToTable("ConsultaRecuperada");
                });

            modelBuilder.Entity("Domain.Entities.ChatAgregado.DocumentoRecuperado", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("DocumentoId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MensajeIAId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Rank")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DocumentoId");

                    b.HasIndex("MensajeIAId");

                    b.ToTable("DocumentoRecuperado");
                });

            modelBuilder.Entity("Domain.Entities.ChatAgregado.Mensaje", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ChatId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.ToTable((string)null);

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("Domain.Entities.ConsultaAgregado.Consulta", b =>
                {
                    b.Property<Guid>("Id")
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

            modelBuilder.Entity("Domain.Entities.DocumentoAgregado.Document", b =>
                {
                    b.Property<Guid>("Id")
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

            modelBuilder.Entity("Domain.Entities.DocumentoAgregado.DocumentChunk", b =>
                {
                    b.Property<Guid>("Id")
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

                    b.ToTable("DocumentChunk");
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

            modelBuilder.Entity("Domain.Entities.ChatAgregado.MensajeIA", b =>
                {
                    b.HasBaseType("Domain.Entities.ChatAgregado.Mensaje");

                    b.Property<bool?>("Calificacion")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.ToTable("MensajeIA");
                });

            modelBuilder.Entity("Domain.Entities.ChatAgregado.MensajeTextoUsuario", b =>
                {
                    b.HasBaseType("Domain.Entities.ChatAgregado.Mensaje");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.ToTable("MensajeTextoUsuario");
                });

            modelBuilder.Entity("Domain.Entities.ChatAgregado.ConsultaRecuperada", b =>
                {
                    b.HasOne("Domain.Entities.ConsultaAgregado.Consulta", null)
                        .WithMany()
                        .HasForeignKey("ConsultaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.ChatAgregado.MensajeIA", null)
                        .WithMany("ConsultasRecuperadas")
                        .HasForeignKey("MensajeIAId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.ChatAgregado.DocumentoRecuperado", b =>
                {
                    b.HasOne("Domain.Entities.DocumentoAgregado.Document", null)
                        .WithMany()
                        .HasForeignKey("DocumentoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.ChatAgregado.MensajeIA", null)
                        .WithMany("DocumentosRecuperados")
                        .HasForeignKey("MensajeIAId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.ChatAgregado.Mensaje", b =>
                {
                    b.HasOne("Domain.Entities.ChatAgregado.Chat", null)
                        .WithMany("Mensajes")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.DocumentoAgregado.DocumentChunk", b =>
                {
                    b.HasOne("Domain.Entities.DocumentoAgregado.Document", null)
                        .WithMany("Chunks")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.ChatAgregado.Chat", b =>
                {
                    b.Navigation("Mensajes");
                });

            modelBuilder.Entity("Domain.Entities.DocumentoAgregado.Document", b =>
                {
                    b.Navigation("Chunks");
                });

            modelBuilder.Entity("Domain.Entities.ChatAgregado.MensajeIA", b =>
                {
                    b.Navigation("ConsultasRecuperadas");

                    b.Navigation("DocumentosRecuperados");
                });
#pragma warning restore 612, 618
        }
    }
}
