using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaraProject1.Migrations
{
    /// <inheritdoc />
    public partial class EventoImagen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Evento");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImagenDatos",
                table: "Evento",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenDatos",
                table: "Evento");

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Evento",
                type: "character varying(500)",
                unicode: false,
                maxLength: 500,
                nullable: true);
        }
    }
}
