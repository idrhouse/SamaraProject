using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SamaraProject1.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullableIdEmprendedor : Migration
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
                    Clave = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__USUARIO__5B65BF97DCF2B1D4", x => x.IdAdministrador);
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
                    ImagenUrl = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evento", x => x.IdEvento);
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
                    IdAdministrador = table.Column<int>(type: "integer", nullable: false),
                    ImagenUrl = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EMPRENDE__F9D7F7E5E4C9D6A3", x => x.IdEmprendedor);
                    table.ForeignKey(
                        name: "FK_Emprendedor_Administrador",
                        column: x => x.IdAdministrador,
                        principalTable: "Administrador",
                        principalColumn: "IdAdministrador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre_Producto = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    ImagenUrl = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    IdEmprendedor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.IdProducto);
                    table.ForeignKey(
                        name: "FK_Productos_Emprendedor",
                        column: x => x.IdEmprendedor,
                        principalTable: "Emprendedor",
                        principalColumn: "IdEmprendedor",
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

            migrationBuilder.CreateIndex(
                name: "IX_Emprendedor_IdAdministrador",
                table: "Emprendedor",
                column: "IdAdministrador");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdEmprendedor",
                table: "Producto",
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
                name: "Evento");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Stands");

            migrationBuilder.DropTable(
                name: "Emprendedor");

            migrationBuilder.DropTable(
                name: "Administrador");
        }
    }
}
