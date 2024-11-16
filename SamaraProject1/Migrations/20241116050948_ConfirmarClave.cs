using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaraProject1.Migrations
{
    /// <inheritdoc />
    public partial class ConfirmarClave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfirmarClave",
                table: "Administrador",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmarClave",
                table: "Administrador");
        }
    }
}
