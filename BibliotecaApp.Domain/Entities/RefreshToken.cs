namespace  BibliotecaApp.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = "";
    public DateTime FechaExpiracion { get; set; }
    public bool EstaRevocado { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}