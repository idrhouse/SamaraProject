using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaraProject1.Migrations
{
    /// <inheritdoc />
    public partial class ProductoImagen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Producto");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImagenDatos",
                table: "Producto",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenDatos",
                table: "Producto");

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Producto",
                type: "character varying(500)",
                unicode: false,
                maxLength: 500,
                nullable: true);
        }
    }
}
