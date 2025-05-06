using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SamaraProject1.Migrations
{
    /// <inheritdoc />
    public partial class SiteSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmarClave",
                table: "Administrador");

            migrationBuilder.CreateTable(
                name: "SiteSetting",
                columns: table => new
                {
                    IdSiteSetting = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SettingKey = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    SettingValue = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSetting", x => x.IdSiteSetting);
                });

            migrationBuilder.InsertData(
                table: "SiteSetting",
                columns: new[] { "IdSiteSetting", "Description", "LastUpdated", "SettingKey", "SettingValue", "UpdatedBy" },
                values: new object[] { 1, "Número de SINPE Móvil para donaciones", new DateTime(2025, 5, 6, 6, 3, 12, 762, DateTimeKind.Utc).AddTicks(1067), "SinpeMovilNumber", "88630334", "Sistema" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteSetting");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmarClave",
                table: "Administrador",
                type: "text",
                nullable: true);
        }
    }
}
