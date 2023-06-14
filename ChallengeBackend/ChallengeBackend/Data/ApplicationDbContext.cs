using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ChallengeBackend.Data.Entities;

namespace ChallengeBackend.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Local> Locals { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<VentaDetalle> VentaDetalles { get; set; }

    public virtual DbSet<Ventum> Venta { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Local>(entity =>
        {
            entity.HasKey(e => e.IdLocal).HasName("PK__Local__3E34B29DF6370FC0");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.IdMarca).HasName("PK__Marca__9B8F8DB2325A25B9");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__9B4120E21FBD1C85");

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.Productos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Producto__ID_Mar__52593CB8");
        });

        modelBuilder.Entity<VentaDetalle>(entity =>
        {
            entity.HasKey(e => e.IdVentaDetalle).HasName("PK__VentaDet__2F0CE38B52091CC3");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.VentaDetalles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VentaDeta__ID_Pr__5DCAEF64");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.VentaDetalles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VentaDeta__ID_Ve__5CD6CB2B");
        });

        modelBuilder.Entity<Ventum>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__Venta__3CD842E5A3F1C767");

            entity.HasOne(d => d.IdLocalNavigation).WithMany(p => p.Venta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Venta__ID_Local__571DF1D5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
