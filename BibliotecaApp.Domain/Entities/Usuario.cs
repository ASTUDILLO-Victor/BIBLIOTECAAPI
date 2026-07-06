namespace BibliotecaApp.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public DateTime? UltimoLogin { get; set; }
    public List<UsuarioRol> UsuarioRoles { get; set; } = new();
    public List<RefreshToken> RefreshTokens { get; set; } = new();
    public List<Prestamo> Prestamos { get; set; } = new();
}