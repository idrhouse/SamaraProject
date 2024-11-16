using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaraProject1.Migrations
{
    /// <inheritdoc />
    public partial class ElimiarRelacionAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emprendedor_Administrador",
                table: "Emprendedor");

            migrationBuilder.DropIndex(
                name: "IX_Emprendedor_IdAdministrador",
                table: "Emprendedor");

            migrationBuilder.DropColumn(
                name: "IdAdministrador",
                table: "Emprendedor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdAdministrador",
                table: "Emprendedor",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Emprendedor_IdAdministrador",
                table: "Emprendedor",
                column: "IdAdministrador");

            migrationBuilder.AddForeignKey(
                name: "FK_Emprendedor_Administrador",
                table: "Emprendedor",
                column: "IdAdministrador",
                principalTable: "Administrador",
                principalColumn: "IdAdministrador",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
