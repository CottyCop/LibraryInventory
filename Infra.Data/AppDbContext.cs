using System;
using System.Collections.Generic;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<autore> autores { get; set; }

    public virtual DbSet<editoriale> editoriales { get; set; }

    public virtual DbSet<libro> libros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<autore>(entity =>
        {
            entity.Property(e => e.id).ValueGeneratedNever();

            entity.HasMany(d => d.libros_ISBNs).WithMany(p => p.autores)
                .UsingEntity<Dictionary<string, object>>(
                    "autores_has_libro",
                    r => r.HasOne<libro>().WithMany()
                        .HasForeignKey("libros_ISBN")
                        .HasConstraintName("FK_ahl_libros"),
                    l => l.HasOne<autore>().WithMany()
                        .HasForeignKey("autores_id")
                        .HasConstraintName("FK_ahl_autores"),
                    j =>
                    {
                        j.HasKey("autores_id", "libros_ISBN");
                        j.ToTable("autores_has_libros");
                    });
        });

        modelBuilder.Entity<editoriale>(entity =>
        {
            entity.Property(e => e.id).ValueGeneratedNever();
        });

        modelBuilder.Entity<libro>(entity =>
        {
            entity.Property(e => e.ISBN).ValueGeneratedNever();

            entity.HasOne(d => d.editoriales).WithMany(p => p.libros)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_libros_editoriales");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
