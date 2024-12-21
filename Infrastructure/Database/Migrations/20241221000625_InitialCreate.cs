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
            migrationBuilder.CreateSequence(
                name: "MensajeSequence");

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChatPlataformaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Plataforma = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MensajesDeTexto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [MensajeSequence]"),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesDeTexto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajesDeTexto_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UsuarioId_ChatPlataformaId_Plataforma",
                table: "Chats",
                columns: new[] { "UsuarioId", "ChatPlataformaId", "Plataforma" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MensajesDeTexto_ChatId",
                table: "MensajesDeTexto",
                column: "ChatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MensajesDeTexto");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropSequence(
                name: "MensajeSequence");
        }
    }
}
