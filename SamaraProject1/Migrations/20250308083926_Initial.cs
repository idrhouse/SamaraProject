using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SamaraProject1.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administrador",
                columns: table => new
                {
                    IdAdministrador = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreAdministrador = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    Apellido = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    Correo = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    Clave = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    ConfirmarClave = table.Column<string>(type: "text", nullable: true),
                    TokenRecuperacion = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    TokenExpiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__USUARIO__5B65BF97DCF2B1D4", x => x.IdAdministrador);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    IdCategoria = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreCategoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.IdCategoria);
                });

            migrationBuilder.CreateTable(
                name: "Evento",
                columns: table => new
                {
                    IdEvento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ImagenDatos = table.Column<byte[]>(type: "bytea", nullable: true),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evento", x => x.IdEvento);
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

            migrationBuilder.CreateTable(
                name: "Emprendedor",
                columns: table => new
                {
                    IdEmprendedor = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreEmprendedor = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    NombreNegocio = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    DescripcionNegocio = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    Correo = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    ImagenDatos = table.Column<byte[]>(type: "bytea", nullable: true),
                    IdCategoria = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EMPRENDE__F9D7F7E5E4C9D6A3", x => x.IdEmprendedor);
                    table.ForeignKey(
                        name: "FK_Emprendedor_Categorias_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre_Producto = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: false),
                    ImagenDatos = table.Column<byte[]>(type: "bytea", nullable: true),
                    IdTipoProducto = table.Column<int>(type: "integer", nullable: false),
                    EmprendedorIdEmprendedor = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.IdProducto);
                    table.ForeignKey(
                        name: "FK_Producto_Emprendedor_EmprendedorIdEmprendedor",
                        column: x => x.EmprendedorIdEmprendedor,
                        principalTable: "Emprendedor",
                        principalColumn: "IdEmprendedor");
                    table.ForeignKey(
                        name: "FK_Producto_TipoProducto_IdTipoProducto",
                        column: x => x.IdTipoProducto,
                        principalTable: "TipoProducto",
                        principalColumn: "IdTipoProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stands",
                columns: table => new
                {
                    IdStand = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero_Stand = table.Column<int>(type: "integer", nullable: false),
                    Descripcion_Stand = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    Disponible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IdEmprendedor = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stands", x => x.IdStand);
                    table.ForeignKey(
                        name: "FK_Stands_Emprendedor",
                        column: x => x.IdEmprendedor,
                        principalTable: "Emprendedor",
                        principalColumn: "IdEmprendedor");
                });

            migrationBuilder.CreateTable(
                name: "ProductoEmprendedores",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "integer", nullable: false),
                    IdEmprendedor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoEmprendedores", x => new { x.IdProducto, x.IdEmprendedor });
                    table.ForeignKey(
                        name: "FK_ProductoEmprendedores_Emprendedor_IdEmprendedor",
                        column: x => x.IdEmprendedor,
                        principalTable: "Emprendedor",
                        principalColumn: "IdEmprendedor",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoEmprendedores_Producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emprendedor_IdCategoria",
                table: "Emprendedor",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_EmprendedorIdEmprendedor",
                table: "Producto",
                column: "EmprendedorIdEmprendedor");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdTipoProducto",
                table: "Producto",
                column: "IdTipoProducto");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoEmprendedores_IdEmprendedor",
                table: "ProductoEmprendedores",
                column: "IdEmprendedor");

            migrationBuilder.CreateIndex(
                name: "IX_Stands_IdEmprendedor",
                table: "Stands",
                column: "IdEmprendedor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrador");

            migrationBuilder.DropTable(
                name: "Evento");

            migrationBuilder.DropTable(
                name: "ProductoEmprendedores");

            migrationBuilder.DropTable(
                name: "Stands");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Emprendedor");

            migrationBuilder.DropTable(
                name: "TipoProducto");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
