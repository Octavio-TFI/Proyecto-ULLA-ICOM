using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Embeddings.Migrations
{
    /// <inheritdoc />
    public partial class ParentDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Documents_DocumentId",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "DocumentId",
                table: "Documents",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_DocumentId",
                table: "Documents",
                newName: "IX_Documents_ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Documents_ParentId",
                table: "Documents",
                column: "ParentId",
                principalTable: "Documents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Documents_ParentId",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Documents",
                newName: "DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_ParentId",
                table: "Documents",
                newName: "IX_Documents_DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Documents_DocumentId",
                table: "Documents",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id");
        }
    }
}
