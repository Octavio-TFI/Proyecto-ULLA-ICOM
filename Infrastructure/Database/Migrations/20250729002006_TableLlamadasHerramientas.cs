using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class TableLlamadasHerramientas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultaRecuperada_MensajeHerramienta_MensajeHerramientaId",
                table: "ConsultaRecuperada");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentoRecuperado_MensajeHerramienta_MensajeHerramientaId",
                table: "DocumentoRecuperado");

            migrationBuilder.DropForeignKey(
                name: "FK_MensajeHerramienta_Chats_ChatId",
                table: "MensajeHerramienta");

            migrationBuilder.DropForeignKey(
                name: "FK_MensajeTextoUsuario_Chats_ChatId",
                table: "MensajeTextoUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MensajeTextoUsuario",
                table: "MensajeTextoUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MensajeHerramienta",
                table: "MensajeHerramienta");

            migrationBuilder.RenameTable(
                name: "MensajeTextoUsuario",
                newName: "MensajesTextoUsuario");

            migrationBuilder.RenameTable(
                name: "MensajeHerramienta",
                newName: "MensajesHerramienta");

            migrationBuilder.RenameIndex(
                name: "IX_MensajeTextoUsuario_ChatId",
                table: "MensajesTextoUsuario",
                newName: "IX_MensajesTextoUsuario_ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_MensajeHerramienta_ChatId",
                table: "MensajesHerramienta",
                newName: "IX_MensajesHerramienta_ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MensajesTextoUsuario",
                table: "MensajesTextoUsuario",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MensajesHerramienta",
                table: "MensajesHerramienta",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MensajesLlamadaHerramienta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlataformaMensajeId = table.Column<string>(type: "TEXT", nullable: true),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChatId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PluginName = table.Column<string>(type: "TEXT", nullable: true),
                    FunctionName = table.Column<string>(type: "TEXT", nullable: false),
                    Argumentos = table.Column<string>(type: "json", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_MensajesLlamadaHerramienta_ChatId",
                table: "MensajesLlamadaHerramienta",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultaRecuperada_MensajesHerramienta_MensajeHerramientaId",
                table: "ConsultaRecuperada",
                column: "MensajeHerramientaId",
                principalTable: "MensajesHerramienta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentoRecuperado_MensajesHerramienta_MensajeHerramientaId",
                table: "DocumentoRecuperado",
                column: "MensajeHerramientaId",
                principalTable: "MensajesHerramienta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MensajesHerramienta_Chats_ChatId",
                table: "MensajesHerramienta",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MensajesTextoUsuario_Chats_ChatId",
                table: "MensajesTextoUsuario",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultaRecuperada_MensajesHerramienta_MensajeHerramientaId",
                table: "ConsultaRecuperada");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentoRecuperado_MensajesHerramienta_MensajeHerramientaId",
                table: "DocumentoRecuperado");

            migrationBuilder.DropForeignKey(
                name: "FK_MensajesHerramienta_Chats_ChatId",
                table: "MensajesHerramienta");

            migrationBuilder.DropForeignKey(
                name: "FK_MensajesTextoUsuario_Chats_ChatId",
                table: "MensajesTextoUsuario");

            migrationBuilder.DropTable(
                name: "MensajesLlamadaHerramienta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MensajesTextoUsuario",
                table: "MensajesTextoUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MensajesHerramienta",
                table: "MensajesHerramienta");

            migrationBuilder.RenameTable(
                name: "MensajesTextoUsuario",
                newName: "MensajeTextoUsuario");

            migrationBuilder.RenameTable(
                name: "MensajesHerramienta",
                newName: "MensajeHerramienta");

            migrationBuilder.RenameIndex(
                name: "IX_MensajesTextoUsuario_ChatId",
                table: "MensajeTextoUsuario",
                newName: "IX_MensajeTextoUsuario_ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_MensajesHerramienta_ChatId",
                table: "MensajeHerramienta",
                newName: "IX_MensajeHerramienta_ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MensajeTextoUsuario",
                table: "MensajeTextoUsuario",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MensajeHerramienta",
                table: "MensajeHerramienta",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultaRecuperada_MensajeHerramienta_MensajeHerramientaId",
                table: "ConsultaRecuperada",
                column: "MensajeHerramientaId",
                principalTable: "MensajeHerramienta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentoRecuperado_MensajeHerramienta_MensajeHerramientaId",
                table: "DocumentoRecuperado",
                column: "MensajeHerramientaId",
                principalTable: "MensajeHerramienta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MensajeHerramienta_Chats_ChatId",
                table: "MensajeHerramienta",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MensajeTextoUsuario_Chats_ChatId",
                table: "MensajeTextoUsuario",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
