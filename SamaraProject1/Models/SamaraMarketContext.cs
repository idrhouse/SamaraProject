using System;
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
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(500)  
                .IsUnicode(false) 
                .IsRequired(false);
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
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(500)
                .IsUnicode(false);

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
                .HasColumnType("datetime");
            entity.Property(e => e.HoraInicio)
                .HasColumnType("time");
            entity.Property(e => e.HoraFin)
                .HasColumnType("time");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

