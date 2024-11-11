using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-ERP3EUJS\\SQLEXPRESS;Database=SamaraMarket;Trusted_Connection=True; TrustServerCertificate=True;");

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
            entity.Property(e => e.IdAdministrador).HasColumnName("IdAdministrador");

            entity.HasOne(d => d.Administrador)
                .WithMany(p => p.Emprendedores)
                .HasForeignKey(d => d.IdAdministrador)
                .HasConstraintName("FK_Emprendedor_Administrador");
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
            entity.Property(e => e.IdEmprendedor);

            entity.HasOne(p => p.Emprendedor)
                .WithMany(e => e.Productos)
                .HasForeignKey(p => p.IdEmprendedor)
                .HasConstraintName("FK_Productos_Emprendedor");
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

            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired(false);

            entity.Property(e => e.IdEmprendedor)
                .IsRequired();

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
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

