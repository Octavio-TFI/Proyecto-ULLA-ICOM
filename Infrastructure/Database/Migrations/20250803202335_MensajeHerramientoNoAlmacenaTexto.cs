using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class MensajeHerramientoNoAlmacenaTexto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultaRecuperada_MensajesHerramienta_MensajeHerramientaId",
                table: "ConsultaRecuperada");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentoRecuperado_MensajesHerramienta_MensajeHerramientaId",
                table: "DocumentoRecuperado");

            migrationBuilder.DropTable(
                name: "MensajesHerramienta");

            migrationBuilder.RenameColumn(
                name: "MensajeHerramientaId",
                table: "DocumentoRecuperado",
                newName: "MensajeHerramientaInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentoRecuperado_MensajeHerramientaId",
                table: "DocumentoRecuperado",
                newName: "IX_DocumentoRecuperado_MensajeHerramientaInfoId");

            migrationBuilder.RenameColumn(
                name: "MensajeHerramientaId",
                table: "ConsultaRecuperada",
                newName: "MensajeHerramientaInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsultaRecuperada_MensajeHerramientaId",
                table: "ConsultaRecuperada",
                newName: "IX_ConsultaRecuperada_MensajeHerramientaInfoId");

            migrationBuilder.CreateTable(
                name: "MensajeHerramientaInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlataformaMensajeId = table.Column<string>(type: "TEXT", nullable: true),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChatId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LlamadaId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MensajeHerramientaInfo_ChatId",
                table: "MensajeHerramientaInfo",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_MensajeHerramientaInfo_LlamadaId",
                table: "MensajeHerramientaInfo",
                column: "LlamadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultaRecuperada_MensajeHerramientaInfo_MensajeHerramientaInfoId",
                table: "ConsultaRecuperada",
                column: "MensajeHerramientaInfoId",
                principalTable: "MensajeHerramientaInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentoRecuperado_MensajeHerramientaInfo_MensajeHerramientaInfoId",
                table: "DocumentoRecuperado",
                column: "MensajeHerramientaInfoId",
                principalTable: "MensajeHerramientaInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultaRecuperada_MensajeHerramientaInfo_MensajeHerramientaInfoId",
                table: "ConsultaRecuperada");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentoRecuperado_MensajeHerramientaInfo_MensajeHerramientaInfoId",
                table: "DocumentoRecuperado");

            migrationBuilder.DropTable(
                name: "MensajeHerramientaInfo");

            migrationBuilder.RenameColumn(
                name: "MensajeHerramientaInfoId",
                table: "DocumentoRecuperado",
                newName: "MensajeHerramientaId");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentoRecuperado_MensajeHerramientaInfoId",
                table: "DocumentoRecuperado",
                newName: "IX_DocumentoRecuperado_MensajeHerramientaId");

            migrationBuilder.RenameColumn(
                name: "MensajeHerramientaInfoId",
                table: "ConsultaRecuperada",
                newName: "MensajeHerramientaId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsultaRecuperada_MensajeHerramientaInfoId",
                table: "ConsultaRecuperada",
                newName: "IX_ConsultaRecuperada_MensajeHerramientaId");

            migrationBuilder.CreateTable(
                name: "MensajesHerramienta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChatId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PlataformaMensajeId = table.Column<string>(type: "TEXT", nullable: true),
                    Texto = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesHerramienta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajesHerramienta_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MensajesHerramienta_ChatId",
                table: "MensajesHerramienta",
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
        }
    }
}
