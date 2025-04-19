using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class PlataformaMensajeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlataformaMensajeId",
                table: "MensajeTextoUsuario",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlataformaMensajeId",
                table: "MensajeIA",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlataformaMensajeId",
                table: "MensajeTextoUsuario");

            migrationBuilder.DropColumn(
                name: "PlataformaMensajeId",
                table: "MensajeIA");
        }
    }
}
