using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Permiso> Permisos { get; set; }
    public DbSet<UsuarioRol> UsuarioRoles { get; set; }
    public DbSet<RolPermiso> RolPermisos { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<TokenBlacklist> TokenBlacklist { get; set; }
    public DbSet<Autor> Autores { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Libro> Libros { get; set; }
    public DbSet<LibroAutor> LibroAutores { get; set; }
    public DbSet<Prestamo> Prestamos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Claves compuestas Auth
        modelBuilder.Entity<UsuarioRol>()
            .HasKey(ur => new { ur.UsuarioId, ur.RolId });

        modelBuilder.Entity<RolPermiso>()
            .HasKey(rp => new { rp.RolId, rp.PermisoId });

        // Clave compuesta LibroAutor
        modelBuilder.Entity<LibroAutor>()
            .HasKey(la => new { la.LibroId, la.AutorId });
    }
}