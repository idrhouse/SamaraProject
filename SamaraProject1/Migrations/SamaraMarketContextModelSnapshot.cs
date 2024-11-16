﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SamaraProject1.Models;

#nullable disable

namespace SamaraProject1.Migrations
{
    [DbContext(typeof(SamaraMarketContext))]
    partial class SamaraMarketContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SamaraProject1.Models.Administrador", b =>
                {
                    b.Property<int>("IdAdministrador")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdAdministrador"));

                    b.Property<string>("Apellido")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Clave")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Correo")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("NombreAdministrador")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.HasKey("IdAdministrador")
                        .HasName("PK__USUARIO__5B65BF97DCF2B1D4");

                    b.ToTable("Administrador", (string)null);
                });

            modelBuilder.Entity("SamaraProject1.Models.Emprendedor", b =>
                {
                    b.Property<int>("IdEmprendedor")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdEmprendedor"));

                    b.Property<string>("Apellidos")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Correo")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("DescripcionNegocio")
                        .HasMaxLength(200)
                        .IsUnicode(false)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("IdAdministrador")
                        .HasColumnType("integer")
                        .HasColumnName("IdAdministrador");

                    b.Property<string>("ImagenUrl")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("NombreEmprendedor")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("NombreNegocio")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Telefono")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.HasKey("IdEmprendedor")
                        .HasName("PK__EMPRENDE__F9D7F7E5E4C9D6A3");

                    b.HasIndex("IdAdministrador");

                    b.ToTable("Emprendedor", (string)null);
                });

            modelBuilder.Entity("SamaraProject1.Models.Evento", b =>
                {
                    b.Property<int>("IdEvento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdEvento"));

                    b.Property<string>("Descripcion")
                        .HasMaxLength(200)
                        .IsUnicode(false)
                        .HasColumnType("character varying(200)");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime");

                    b.Property<string>("ImagenUrl")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Nombre")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.HasKey("IdEvento");

                    b.ToTable("Evento", (string)null);
                });

            modelBuilder.Entity("SamaraProject1.Models.Producto", b =>
                {
                    b.Property<int>("IdProducto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdProducto"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("character varying(500)");

                    b.Property<int?>("EmprendedorIdEmprendedor")
                        .HasColumnType("integer");

                    b.Property<int?>("IdTipoProducto")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<string>("ImagenUrl")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Nombre_Producto")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)");

                    b.HasKey("IdProducto");

                    b.HasIndex("EmprendedorIdEmprendedor");

                    b.HasIndex("IdTipoProducto");

                    b.ToTable("Producto", (string)null);
                });

            modelBuilder.Entity("SamaraProject1.Models.ProductoEmprendedor", b =>
                {
                    b.Property<int>("IdProducto")
                        .HasColumnType("integer");

                    b.Property<int>("IdEmprendedor")
                        .HasColumnType("integer");

                    b.HasKey("IdProducto", "IdEmprendedor");

                    b.HasIndex("IdEmprendedor");

                    b.ToTable("ProductoEmprendedores");
                });

            modelBuilder.Entity("SamaraProject1.Models.Stands", b =>
                {
                    b.Property<int>("IdStand")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdStand"));

                    b.Property<string>("Descripcion_Stand")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("character varying(500)");

                    b.Property<bool>("Disponible")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<int?>("IdEmprendedor")
                        .HasColumnType("integer");

                    b.Property<int>("Numero_Stand")
                        .HasColumnType("integer");

                    b.HasKey("IdStand");

                    b.HasIndex("IdEmprendedor");

                    b.ToTable("Stands", (string)null);
                });

            modelBuilder.Entity("SamaraProject1.Models.TipoProducto", b =>
                {
                    b.Property<int>("IdTipoProducto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdTipoProducto"));

                    b.Property<string>("NombreTipo")
                        .HasColumnType("text");

                    b.HasKey("IdTipoProducto");

                    b.ToTable("TipoProducto");
                });

            modelBuilder.Entity("SamaraProject1.Models.Emprendedor", b =>
                {
                    b.HasOne("SamaraProject1.Models.Administrador", "Administrador")
                        .WithMany("Emprendedores")
                        .HasForeignKey("IdAdministrador")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Emprendedor_Administrador");

                    b.Navigation("Administrador");
                });

            modelBuilder.Entity("SamaraProject1.Models.Producto", b =>
                {
                    b.HasOne("SamaraProject1.Models.Emprendedor", null)
                        .WithMany("Productos")
                        .HasForeignKey("EmprendedorIdEmprendedor");

                    b.HasOne("SamaraProject1.Models.TipoProducto", "TipoProducto")
                        .WithMany("Productos")
                        .HasForeignKey("IdTipoProducto")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TipoProducto");
                });

            modelBuilder.Entity("SamaraProject1.Models.ProductoEmprendedor", b =>
                {
                    b.HasOne("SamaraProject1.Models.Emprendedor", "Emprendedor")
                        .WithMany("ProductoEmprendedores")
                        .HasForeignKey("IdEmprendedor")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SamaraProject1.Models.Producto", "Producto")
                        .WithMany("ProductoEmprendedores")
                        .HasForeignKey("IdProducto")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Emprendedor");

                    b.Navigation("Producto");
                });

            modelBuilder.Entity("SamaraProject1.Models.Stands", b =>
                {
                    b.HasOne("SamaraProject1.Models.Emprendedor", "Emprendedor")
                        .WithMany("Stands")
                        .HasForeignKey("IdEmprendedor")
                        .HasConstraintName("FK_Stands_Emprendedor");

                    b.Navigation("Emprendedor");
                });

            modelBuilder.Entity("SamaraProject1.Models.Administrador", b =>
                {
                    b.Navigation("Emprendedores");
                });

            modelBuilder.Entity("SamaraProject1.Models.Emprendedor", b =>
                {
                    b.Navigation("ProductoEmprendedores");

                    b.Navigation("Productos");

                    b.Navigation("Stands");
                });

            modelBuilder.Entity("SamaraProject1.Models.Producto", b =>
                {
                    b.Navigation("ProductoEmprendedores");
                });

            modelBuilder.Entity("SamaraProject1.Models.TipoProducto", b =>
                {
                    b.Navigation("Productos");
                });
#pragma warning restore 612, 618
        }
    }
}
