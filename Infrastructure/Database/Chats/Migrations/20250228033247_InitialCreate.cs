using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Infrastructure.Database.Chats.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Chat");

            migrationBuilder.CreateTable(
                name: "Chats",
                schema: "Chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false),
                    UsuarioId = table.Column<string>(
                        type: "nvarchar(450)",
                        nullable: false),
                    ChatPlataformaId = table.Column<string>(
                        type: "nvarchar(450)",
                        nullable: false),
                    Plataforma = table.Column<string>(
                        type: "nvarchar(450)",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxEvents",
                schema: "Chat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventType = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),
                    EventData = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),
                    OccurredOn = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),
                    ProcessedOn = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MensajesDeTexto",
                schema: "Chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false),
                    DateTime = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    ChatId = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false),
                    Texto = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesDeTexto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajesDeTexto_Chats_ChatId",
                        column: x => x.ChatId,
                        principalSchema: "Chat",
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UsuarioId_ChatPlataformaId_Plataforma",
                schema: "Chat",
                table: "Chats",
                columns: new[] { "UsuarioId", "ChatPlataformaId", "Plataforma" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MensajesDeTexto_ChatId",
                schema: "Chat",
                table: "MensajesDeTexto",
                column: "ChatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MensajesDeTexto",
                schema: "Chat");

            migrationBuilder.DropTable(
                name: "OutboxEvents",
                schema: "Chat");

            migrationBuilder.DropTable(
                name: "Chats",
                schema: "Chat");
        }
    }
}
