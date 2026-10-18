using MangaHub.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaHub.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Autor> Autores => Set<Autor>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Manga> Mangas => Set<Manga>();
    public DbSet<EntradaBiblioteca> Biblioteca => Set<EntradaBiblioteca>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.Property(u => u.Email).HasMaxLength(100).IsRequired();
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Rol).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<Autor>(e =>
        {
            e.Property(a => a.Nombre).HasMaxLength(100).IsRequired();
            e.Property(a => a.Nacionalidad).HasMaxLength(50);
        });

        modelBuilder.Entity<Categoria>(e =>
        {
            e.Property(c => c.Nombre).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<Manga>(e =>
        {
            e.Property(m => m.Nombre).HasMaxLength(150).IsRequired();
            e.HasOne(m => m.Autor)
                .WithMany(a => a.Mangas)
                .HasForeignKey(m => m.AutorId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(m => m.Categoria)
                .WithMany(c => c.Mangas)
                .HasForeignKey(m => m.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EntradaBiblioteca>(e =>
        {
            e.HasKey(x => new { x.UsuarioId, x.MangaId });
            e.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
            e.HasOne(x => x.Usuario)
                .WithMany(u => u.Biblioteca)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Manga)
                .WithMany()
                .HasForeignKey(x => x.MangaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Datos iniciales (seed)
        modelBuilder.Entity<Autor>().HasData(
            new Autor { Id = 1, Nombre = "Eiichiro Oda", Nacionalidad = "Japonesa" },
            new Autor { Id = 2, Nombre = "Hajime Isayama", Nacionalidad = "Japonesa" },
            new Autor { Id = 3, Nombre = "Naoki Urasawa", Nacionalidad = "Japonesa" }
        );

        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Shonen", Descripcion = "Dirigido a un público juvenil masculino" },
            new Categoria { Id = 2, Nombre = "Seinen", Descripcion = "Dirigido a un público adulto" },
            new Categoria { Id = 3, Nombre = "Shojo", Descripcion = "Dirigido a un público juvenil femenino" }
        );

        modelBuilder.Entity<Manga>().HasData(
            new Manga { Id = 1, Nombre = "One Piece", Resumen = "Un joven pirata busca el tesoro más grande del mundo.", AutorId = 1, CategoriaId = 1 },
            new Manga { Id = 2, Nombre = "Shingeki no Kyojin", Resumen = "La humanidad lucha por sobrevivir contra los titanes.", AutorId = 2, CategoriaId = 1 },
            new Manga { Id = 3, Nombre = "Monster", Resumen = "Un neurocirujano persigue al paciente que salvó y se convirtió en un asesino.", AutorId = 3, CategoriaId = 2 }
        );
    }
}
