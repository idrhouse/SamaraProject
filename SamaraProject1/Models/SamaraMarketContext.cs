﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SamaraProject1.Models;

public partial class SamaraMarketContext : DbContext
{
    public SamaraMarketContext()
    {
    }

    public SamaraMarketContext(DbContextOptions<SamaraMarketContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrador> Administrador { get; set; }
    public virtual DbSet<Emprendedor> Emprendedores { get; set; }
    public virtual DbSet<Producto> Productos { get; set; }
    public virtual DbSet<Stands> Stands { get; set; }
    public virtual DbSet<Evento> Eventos { get; set; }
    public virtual DbSet<TipoProducto> TipoProducto { get; set; }
    public virtual DbSet<ProductoEmprendedor> ProductoEmprendedores { get; set; }
    public virtual DbSet<Categoria> Categorias { get; set; }
    public virtual DbSet<CarouselImage> CarouselImages { get; set; }
    public virtual DbSet<Testimonio> Testimonios { get; set; }
    public virtual DbSet<SiteSetting> SiteSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.HasKey(e => e.IdAdministrador).HasName("PK__USUARIO__5B65BF97DCF2B1D4");

            entity.ToTable("Administrador");

            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Clave)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NombreAdministrador)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TokenRecuperacion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsRequired(false);
        });

        modelBuilder.Entity<Emprendedor>(entity =>
        {
            entity.HasKey(e => e.IdEmprendedor).HasName("PK__EMPRENDE__F9D7F7E5E4C9D6A3");

            entity.ToTable("Emprendedor");

            entity.Property(e => e.NombreEmprendedor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Apellidos)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NombreNegocio)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DescripcionNegocio)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ImagenDatos)
                .HasColumnType("bytea");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto);

            entity.ToTable("Producto");

            entity.Property(e => e.Nombre_Producto)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ImagenDatos)
                .HasColumnType("bytea");

            entity.HasOne(p => p.TipoProducto)
                .WithMany(t => t.Productos)
                .HasForeignKey(p => p.IdTipoProducto);
        });

        modelBuilder.Entity<Stands>(entity =>
        {
            entity.HasKey(e => e.IdStand);

            entity.ToTable("Stands");

            entity.Property(e => e.IdStand)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Numero_Stand)
                .IsRequired();

            entity.Property(e => e.Descripcion_Stand)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired(false);

            entity.Property(e => e.Disponible)
                .HasDefaultValue(true);

            entity.Property(e => e.IdEmprendedor)
                .IsRequired(false);

            entity.HasOne(d => d.Emprendedor)
                .WithMany(p => p.Stands)
                .HasForeignKey(d => d.IdEmprendedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Stands_Emprendedor");
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.IdEvento);

            entity.ToTable("Evento");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Fecha)
                .HasColumnType("timestamp with time zone")
                .IsRequired();
            entity.Property(e => e.HoraInicio)
                .HasColumnType("time")
                .IsRequired();
            entity.Property(e => e.HoraFin)
                .HasColumnType("time")
                .IsRequired();
            entity.Property(e => e.ImagenDatos)
                .HasColumnType("bytea");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(c => c.IdCategoria);
            entity.Property(c => c.IdCategoria).ValueGeneratedOnAdd();
            entity.Property(c => c.NombreCategoria).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<CarouselImage>(entity =>
        {
            entity.HasKey(e => e.IdCarouselImage);
            entity.ToTable("CarouselImages"); // Cambiado a plural

            entity.Property(e => e.IdCarouselImage)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Url)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.Alt)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.Order)
                .HasColumnName("OrderNumber"); // Evitar palabra reservada

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired(false);
        });

        // Relación 1 a muchos: Producto - TipoProducto
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.TipoProducto)
            .WithMany(t => t.Productos)
            .HasForeignKey(p => p.IdTipoProducto);

        // Relación muchos a muchos: Producto - Emprendedor (tabla intermedia)
        modelBuilder.Entity<ProductoEmprendedor>()
            .HasKey(pe => new { pe.IdProducto, pe.IdEmprendedor });

        modelBuilder.Entity<ProductoEmprendedor>()
            .HasOne(pe => pe.Producto)
            .WithMany(p => p.ProductoEmprendedores)
            .HasForeignKey(pe => pe.IdProducto);

        modelBuilder.Entity<ProductoEmprendedor>()
            .HasOne(pe => pe.Emprendedor)
            .WithMany(e => e.ProductoEmprendedores)
            .HasForeignKey(pe => pe.IdEmprendedor);

        //TipoProducto
        modelBuilder.Entity<TipoProducto>()
            .HasKey(t => t.IdTipoProducto);

        modelBuilder.Entity<Emprendedor>()
            .HasOne(e => e.Categoria)
            .WithMany(c => c.Emprendedores)
            .HasForeignKey(e => e.IdCategoria)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Testimonio>(entity =>
        {
            entity.HasKey(e => e.IdTestimonio);
            entity.ToTable("Testimonio");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Contenido)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.ProductoFavorito)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsRequired(false);
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<SiteSetting>(entity =>
        {
            entity.HasKey(e => e.IdSiteSetting);
            entity.ToTable("SiteSetting");

            entity.Property(e => e.IdSiteSetting)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.SettingKey)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.SettingValue)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.Property(e => e.LastUpdated)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .IsUnicode(false);
        });
        // Datos iniciales para SiteSettings
        modelBuilder.Entity<SiteSetting>().HasData(
            new SiteSetting
            {
                IdSiteSetting = 1,
                SettingKey = "SinpeMovilNumber",
                SettingValue = "88630334",
                Description = "Número de SINPE Móvil para donaciones",
                LastUpdated = DateTime.UtcNow,
                UpdatedBy = "Sistema"
            }
        );



        OnModelCreatingPartial(modelBuilder);
    }



    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}