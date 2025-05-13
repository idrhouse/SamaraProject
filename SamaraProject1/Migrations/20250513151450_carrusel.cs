using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaraProject1.Migrations
{
    /// <inheritdoc />
    public partial class carrusel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SiteSetting",
                keyColumn: "IdSiteSetting",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2025, 5, 13, 15, 14, 48, 837, DateTimeKind.Utc).AddTicks(4954));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SiteSetting",
                keyColumn: "IdSiteSetting",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2025, 5, 6, 6, 3, 12, 762, DateTimeKind.Utc).AddTicks(1067));
        }
    }
}
