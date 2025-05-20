using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaraProject1.Migrations
{
    /// <inheritdoc />
    public partial class FixCarouselImageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CarouselImage",
                table: "CarouselImage");

            migrationBuilder.RenameTable(
                name: "CarouselImage",
                newName: "CarouselImages");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "CarouselImages",
                newName: "OrderNumber");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CarouselImages",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarouselImages",
                table: "CarouselImages",
                column: "IdCarouselImage");

            migrationBuilder.UpdateData(
                table: "SiteSetting",
                keyColumn: "IdSiteSetting",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2025, 5, 13, 15, 55, 5, 733, DateTimeKind.Utc).AddTicks(5252));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CarouselImages",
                table: "CarouselImages");

            migrationBuilder.RenameTable(
                name: "CarouselImages",
                newName: "CarouselImage");

            migrationBuilder.RenameColumn(
                name: "OrderNumber",
                table: "CarouselImage",
                newName: "Order");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CarouselImage",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarouselImage",
                table: "CarouselImage",
                column: "IdCarouselImage");

            migrationBuilder.UpdateData(
                table: "SiteSetting",
                keyColumn: "IdSiteSetting",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2025, 5, 13, 15, 14, 48, 837, DateTimeKind.Utc).AddTicks(4954));
        }
    }
}
