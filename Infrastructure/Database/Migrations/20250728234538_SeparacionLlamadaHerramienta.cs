using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeparacionLlamadaHerramienta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultaRecuperada_MensajeIA_MensajeIAId",
                table: "ConsultaRecuperada");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentoRecuperado_MensajeIA_MensajeIAId",
                table: "DocumentoRecuperado");

            migrationBuilder.DropForeignKey(
                name: "FK_MensajeIA_Chats_ChatId",
                table: "MensajeIA");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MensajeIA",
                table: "MensajeIA");

            migrationBuilder.RenameTable(
                name: "MensajeIA",
                newName: "MensajesIA");

            migrationBuilder.RenameColumn(
                name: "MensajeIAId",
                table: "DocumentoRecuperado",
                newName: "MensajeHerramientaId");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentoRecuperado_MensajeIAId",
                table: "DocumentoRecuperado",
                newName: "IX_DocumentoRecuperado_MensajeHerramientaId");

            migrationBuilder.RenameColumn(
                name: "MensajeIAId",
                table: "ConsultaRecuperada",
                newName: "MensajeHerramientaId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsultaRecuperada_MensajeIAId",
                table: "ConsultaRecuperada",
                newName: "IX_ConsultaRecuperada_MensajeHerramientaId");

            migrationBuilder.RenameIndex(
                name: "IX_MensajeIA_ChatId",
                table: "MensajesIA",
                newName: "IX_MensajesIA_ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MensajesIA",
                table: "MensajesIA",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MensajeHerramienta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlataformaMensajeId = table.Column<string>(type: "TEXT", nullable: true),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChatId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Texto = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajeHerramienta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajeHerramienta_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MensajeHerramienta_ChatId",
                table: "MensajeHerramienta",
                column: "ChatId");

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
                name: "FK_MensajesIA_Chats_ChatId",
                table: "MensajesIA",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultaRecuperada_MensajeHerramienta_MensajeHerramientaId",
                table: "ConsultaRecuperada");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentoRecuperado_MensajeHerramienta_MensajeHerramientaId",
                table: "DocumentoRecuperado");

            migrationBuilder.DropForeignKey(
                name: "FK_MensajesIA_Chats_ChatId",
                table: "MensajesIA");

            migrationBuilder.DropTable(
                name: "MensajeHerramienta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MensajesIA",
                table: "MensajesIA");

            migrationBuilder.RenameTable(
                name: "MensajesIA",
                newName: "MensajeIA");

            migrationBuilder.RenameColumn(
                name: "MensajeHerramientaId",
                table: "DocumentoRecuperado",
                newName: "MensajeIAId");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentoRecuperado_MensajeHerramientaId",
                table: "DocumentoRecuperado",
                newName: "IX_DocumentoRecuperado_MensajeIAId");

            migrationBuilder.RenameColumn(
                name: "MensajeHerramientaId",
                table: "ConsultaRecuperada",
                newName: "MensajeIAId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsultaRecuperada_MensajeHerramientaId",
                table: "ConsultaRecuperada",
                newName: "IX_ConsultaRecuperada_MensajeIAId");

            migrationBuilder.RenameIndex(
                name: "IX_MensajesIA_ChatId",
                table: "MensajeIA",
                newName: "IX_MensajeIA_ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MensajeIA",
                table: "MensajeIA",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultaRecuperada_MensajeIA_MensajeIAId",
                table: "ConsultaRecuperada",
                column: "MensajeIAId",
                principalTable: "MensajeIA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentoRecuperado_MensajeIA_MensajeIAId",
                table: "DocumentoRecuperado",
                column: "MensajeIAId",
                principalTable: "MensajeIA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MensajeIA_Chats_ChatId",
                table: "MensajeIA",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
