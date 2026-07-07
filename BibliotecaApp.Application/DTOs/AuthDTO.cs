namespace BibliotecaApp.Application.DTOs;

public class RegistroDTO
{
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Password { get; set; } = "";
    public string ConfirmarPassword { get; set; } = "";
}

public class LoginDTO
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class AuthResponseDTO
{
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public string Rol { get; set; } = "";
    public DateTime? UltimoLogin { get; set; }
}

public class RefreshTokenDTO
{
    public string RefreshToken { get; set; } = "";
}

public class PerfilResponseDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Rol { get; set; } = "";
    public DateTime FechaRegistro { get; set; }
    public DateTime? UltimoLogin { get; set; }
}