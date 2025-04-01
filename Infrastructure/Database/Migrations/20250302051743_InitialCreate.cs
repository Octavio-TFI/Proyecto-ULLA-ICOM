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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: false),
                    ChatPlataformaId = table.Column<string>(type: "TEXT", nullable: false),
                    Plataforma = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Consultas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RemoteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    Solucion = table.Column<string>(type: "TEXT", nullable: false),
                    EmbeddingTitulo = table.Column<string>(type: "float[768]", nullable: false),
                    EmbeddingDescripcion = table.Column<string>(type: "float[768]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Filename = table.Column<string>(type: "TEXT", nullable: false),
                    Texto = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventType = table.Column<string>(type: "TEXT", nullable: false),
                    EventData = table.Column<string>(type: "TEXT", nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProcessedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsProcessed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MensajeIA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChatId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Texto = table.Column<string>(type: "TEXT", nullable: false),
                    Calificacion = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajeIA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajeIA_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MensajeTextoUsuario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChatId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Texto = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajeTextoUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajeTextoUsuario_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentChunk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Texto = table.Column<string>(type: "TEXT", nullable: false),
                    Embedding = table.Column<string>(type: "float[768]", nullable: false),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                name: "ConsultaRecuperada",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConsultaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Rank = table.Column<bool>(type: "INTEGER", nullable: false),
                    MensajeIAId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                        name: "FK_ConsultaRecuperada_MensajeIA_MensajeIAId",
                        column: x => x.MensajeIAId,
                        principalTable: "MensajeIA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoRecuperado",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DocumentoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Rank = table.Column<bool>(type: "INTEGER", nullable: false),
                    MensajeIAId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                        name: "FK_DocumentoRecuperado_MensajeIA_MensajeIAId",
                        column: x => x.MensajeIAId,
                        principalTable: "MensajeIA",
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
                name: "IX_ConsultaRecuperada_MensajeIAId",
                table: "ConsultaRecuperada",
                column: "MensajeIAId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentChunk_DocumentId",
                table: "DocumentChunk",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoRecuperado_DocumentoId",
                table: "DocumentoRecuperado",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoRecuperado_MensajeIAId",
                table: "DocumentoRecuperado",
                column: "MensajeIAId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Filename",
                table: "Documents",
                column: "Filename",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MensajeIA_ChatId",
                table: "MensajeIA",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_MensajeTextoUsuario_ChatId",
                table: "MensajeTextoUsuario",
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
                name: "MensajeTextoUsuario");

            migrationBuilder.DropTable(
                name: "OutboxEvents");

            migrationBuilder.DropTable(
                name: "Consultas");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "MensajeIA");

            migrationBuilder.DropTable(
                name: "Chats");
        }
    }
}
