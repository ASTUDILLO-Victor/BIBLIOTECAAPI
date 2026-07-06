namespace BibliotecaApp.Domain.Entities;

public class TokenBlacklist
{
    public int Id { get; set; }
    public string Token { get; set; } = "";
    public DateTime FechaExpiracion { get; set; }
    public DateTime FechaAgregado { get; set; } = DateTime.UtcNow;
}