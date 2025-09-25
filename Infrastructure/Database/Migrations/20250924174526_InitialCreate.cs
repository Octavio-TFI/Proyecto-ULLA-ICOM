using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChatPlataformaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Plataforma = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Consultas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RemoteId = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Solucion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmbeddingTitulo = table.Column<string>(type: "vector(768)", nullable: false),
                    EmbeddingDescripcion = table.Column<string>(type: "vector(768)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Filename = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    MaxRetries = table.Column<int>(type: "int", nullable: false),
                    RetryIntervalSeconds = table.Column<double>(type: "float", nullable: false),
                    NextRetryOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MensajesIA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlataformaMensajeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Calificacion = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesIA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajesIA_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MensajesLlamadaHerramienta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlataformaMensajeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PluginName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FunctionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Argumentos = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesLlamadaHerramienta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajesLlamadaHerramienta_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MensajesTextoUsuario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlataformaMensajeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesTextoUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajesTextoUsuario_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentChunk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Embedding = table.Column<string>(type: "vector(768)", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentChunk", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentChunk_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MensajeHerramientaInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlataformaMensajeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LlamadaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajeHerramientaInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajeHerramientaInfo_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MensajeHerramientaInfo_MensajesLlamadaHerramienta_LlamadaId",
                        column: x => x.LlamadaId,
                        principalTable: "MensajesLlamadaHerramienta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsultaRecuperada",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsultaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rank = table.Column<bool>(type: "bit", nullable: false),
                    MensajeHerramientaInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultaRecuperada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultaRecuperada_Consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "Consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsultaRecuperada_MensajeHerramientaInfo_MensajeHerramientaInfoId",
                        column: x => x.MensajeHerramientaInfoId,
                        principalTable: "MensajeHerramientaInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoRecuperado",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rank = table.Column<bool>(type: "bit", nullable: false),
                    MensajeHerramientaInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoRecuperado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentoRecuperado_Documents_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentoRecuperado_MensajeHerramientaInfo_MensajeHerramientaInfoId",
                        column: x => x.MensajeHerramientaInfoId,
                        principalTable: "MensajeHerramientaInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UsuarioId_ChatPlataformaId_Plataforma",
                table: "Chats",
                columns: new[] { "UsuarioId", "ChatPlataformaId", "Plataforma" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsultaRecuperada_ConsultaId",
                table: "ConsultaRecuperada",
                column: "ConsultaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultaRecuperada_MensajeHerramientaInfoId",
                table: "ConsultaRecuperada",
                column: "MensajeHerramientaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentChunk_DocumentId",
                table: "DocumentChunk",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoRecuperado_DocumentoId",
                table: "DocumentoRecuperado",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoRecuperado_MensajeHerramientaInfoId",
                table: "DocumentoRecuperado",
                column: "MensajeHerramientaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Filename",
                table: "Documents",
                column: "Filename",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MensajeHerramientaInfo_ChatId",
                table: "MensajeHerramientaInfo",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_MensajeHerramientaInfo_LlamadaId",
                table: "MensajeHerramientaInfo",
                column: "LlamadaId",
                unique: true,
                filter: "[LlamadaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MensajesIA_ChatId",
                table: "MensajesIA",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_MensajesLlamadaHerramienta_ChatId",
                table: "MensajesLlamadaHerramienta",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_MensajesTextoUsuario_ChatId",
                table: "MensajesTextoUsuario",
                column: "ChatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsultaRecuperada");

            migrationBuilder.DropTable(
                name: "DocumentChunk");

            migrationBuilder.DropTable(
                name: "DocumentoRecuperado");

            migrationBuilder.DropTable(
                name: "MensajesIA");

            migrationBuilder.DropTable(
                name: "MensajesTextoUsuario");

            migrationBuilder.DropTable(
                name: "OutboxEvents");

            migrationBuilder.DropTable(
                name: "Consultas");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "MensajeHerramientaInfo");

            migrationBuilder.DropTable(
                name: "MensajesLlamadaHerramienta");

            migrationBuilder.DropTable(
                name: "Chats");
        }
    }
}
