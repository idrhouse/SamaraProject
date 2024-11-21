using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaraProject1.Migrations
{
    /// <inheritdoc />
    public partial class EmprendedorImagen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Emprendedor");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImagenDatos",
                table: "Emprendedor",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenDatos",
                table: "Emprendedor");

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Emprendedor",
                type: "character varying(500)",
                unicode: false,
                maxLength: 500,
                nullable: true);
        }
    }
}
