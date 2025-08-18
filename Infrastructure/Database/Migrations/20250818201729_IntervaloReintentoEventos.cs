using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class IntervaloReintentoEventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NextRetryOn",
                table: "OutboxEvents",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RetryIntervalSeconds",
                table: "OutboxEvents",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextRetryOn",
                table: "OutboxEvents");

            migrationBuilder.DropColumn(
                name: "RetryIntervalSeconds",
                table: "OutboxEvents");
        }
    }
}
