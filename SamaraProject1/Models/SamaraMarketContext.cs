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

        modelBuilder.Entity<Emprendedor>(entity => { 
        entity.HasKey(e => e.IdEmprendedor).HasName("PK__EMPRENDE__F9D7F7E5E4C9D6A3");

        entity.ToTable("Emprendedor");
        entity.Property(e => e.NombreEmprendedor)
          .HasMaxLength(50)
          .IsUnicode(false);
         entity.Property(e => e.Apellidos) .HasMaxLength(50) .IsUnicode(false);
         entity.Property(e => e.NombreNegocio) .HasMaxLength(50) .IsUnicode(false);
         entity.Property(e =>e.DescripcionNegocio) .HasMaxLength(200) .IsUnicode(false);
         entity.Property(e => e.Telefono) .HasMaxLength(50) .IsUnicode(false);
         entity.Property(e => e.Correo) .HasMaxLength(50) .IsUnicode(false);
         entity.Property(e => e.IdAdministrador).HasColumnName("IdAdministrador");
         entity.HasOne(d => d.Administrador)
            .WithMany(p => p.Emprendedores)
             .HasForeignKey(d => d.IdAdministrador)
             .HasConstraintName("FK_Emprendedor_Administrador");

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
