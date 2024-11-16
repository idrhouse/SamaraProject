using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaraProject1.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producto_TipoProducto_IdTipoProducto",
                table: "Producto");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Emprendedor",
                table: "Producto");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoEmprendedor_Emprendedor_IdEmprendedor",
                table: "ProductoEmprendedor");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoEmprendedor_Producto_IdProducto",
                table: "ProductoEmprendedor");

            migrationBuilder.DropIndex(
                name: "IX_Producto_IdEmprendedor",
                table: "Producto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductoEmprendedor",
                table: "ProductoEmprendedor");

            migrationBuilder.DropColumn(
                name: "IdEmprendedor",
                table: "Producto");

            migrationBuilder.RenameTable(
                name: "ProductoEmprendedor",
                newName: "ProductoEmprendedores");

            migrationBuilder.RenameIndex(
                name: "IX_ProductoEmprendedor_IdEmprendedor",
                table: "ProductoEmprendedores",
                newName: "IX_ProductoEmprendedores_IdEmprendedor");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre_Producto",
                table: "Producto",
                type: "character varying(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdTipoProducto",
                table: "Producto",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Producto",
                type: "character varying(500)",
                unicode: false,
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldUnicode: false,
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmprendedorIdEmprendedor",
                table: "Producto",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductoEmprendedores",
                table: "ProductoEmprendedores",
                columns: new[] { "IdProducto", "IdEmprendedor" });

            migrationBuilder.CreateIndex(
                name: "IX_Producto_EmprendedorIdEmprendedor",
                table: "Producto",
                column: "EmprendedorIdEmprendedor");

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_Emprendedor_EmprendedorIdEmprendedor",
                table: "Producto",
                column: "EmprendedorIdEmprendedor",
                principalTable: "Emprendedor",
                principalColumn: "IdEmprendedor");

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_TipoProducto_IdTipoProducto",
                table: "Producto",
                column: "IdTipoProducto",
                principalTable: "TipoProducto",
                principalColumn: "IdTipoProducto",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoEmprendedores_Emprendedor_IdEmprendedor",
                table: "ProductoEmprendedores",
                column: "IdEmprendedor",
                principalTable: "Emprendedor",
                principalColumn: "IdEmprendedor",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoEmprendedores_Producto_IdProducto",
                table: "ProductoEmprendedores",
                column: "IdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producto_Emprendedor_EmprendedorIdEmprendedor",
                table: "Producto");

            migrationBuilder.DropForeignKey(
                name: "FK_Producto_TipoProducto_IdTipoProducto",
                table: "Producto");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoEmprendedores_Emprendedor_IdEmprendedor",
                table: "ProductoEmprendedores");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoEmprendedores_Producto_IdProducto",
                table: "ProductoEmprendedores");

            migrationBuilder.DropIndex(
                name: "IX_Producto_EmprendedorIdEmprendedor",
                table: "Producto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductoEmprendedores",
                table: "ProductoEmprendedores");

            migrationBuilder.DropColumn(
                name: "EmprendedorIdEmprendedor",
                table: "Producto");

            migrationBuilder.RenameTable(
                name: "ProductoEmprendedores",
                newName: "ProductoEmprendedor");

            migrationBuilder.RenameIndex(
                name: "IX_ProductoEmprendedores_IdEmprendedor",
                table: "ProductoEmprendedor",
                newName: "IX_ProductoEmprendedor_IdEmprendedor");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre_Producto",
                table: "Producto",
                type: "character varying(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldUnicode: false,
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "IdTipoProducto",
                table: "Producto",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Producto",
                type: "character varying(500)",
                unicode: false,
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldUnicode: false,
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "IdEmprendedor",
                table: "Producto",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductoEmprendedor",
                table: "ProductoEmprendedor",
                columns: new[] { "IdProducto", "IdEmprendedor" });

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdEmprendedor",
                table: "Producto",
                column: "IdEmprendedor");

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_TipoProducto_IdTipoProducto",
                table: "Producto",
                column: "IdTipoProducto",
                principalTable: "TipoProducto",
                principalColumn: "IdTipoProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Emprendedor",
                table: "Producto",
                column: "IdEmprendedor",
                principalTable: "Emprendedor",
                principalColumn: "IdEmprendedor",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoEmprendedor_Emprendedor_IdEmprendedor",
                table: "ProductoEmprendedor",
                column: "IdEmprendedor",
                principalTable: "Emprendedor",
                principalColumn: "IdEmprendedor",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoEmprendedor_Producto_IdProducto",
                table: "ProductoEmprendedor",
                column: "IdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
