using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SamaraProject1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdEmprendedor",
                table: "Stands",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "IdTipoProducto",
                table: "Producto",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductoEmprendedor",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "integer", nullable: false),
                    IdEmprendedor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoEmprendedor", x => new { x.IdProducto, x.IdEmprendedor });
                    table.ForeignKey(
                        name: "FK_ProductoEmprendedor_Emprendedor_IdEmprendedor",
                        column: x => x.IdEmprendedor,
                        principalTable: "Emprendedor",
                        principalColumn: "IdEmprendedor",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoEmprendedor_Producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipoProducto",
                columns: table => new
                {
                    IdTipoProducto = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreTipo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoProducto", x => x.IdTipoProducto);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdTipoProducto",
                table: "Producto",
                column: "IdTipoProducto");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoEmprendedor_IdEmprendedor",
                table: "ProductoEmprendedor",
                column: "IdEmprendedor");

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_TipoProducto_IdTipoProducto",
                table: "Producto",
                column: "IdTipoProducto",
                principalTable: "TipoProducto",
                principalColumn: "IdTipoProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producto_TipoProducto_IdTipoProducto",
                table: "Producto");

            migrationBuilder.DropTable(
                name: "ProductoEmprendedor");

            migrationBuilder.DropTable(
                name: "TipoProducto");

            migrationBuilder.DropIndex(
                name: "IX_Producto_IdTipoProducto",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "IdTipoProducto",
                table: "Producto");

            migrationBuilder.AlterColumn<int>(
                name: "IdEmprendedor",
                table: "Stands",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
