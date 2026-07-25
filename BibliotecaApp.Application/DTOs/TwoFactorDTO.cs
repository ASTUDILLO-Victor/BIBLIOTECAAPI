namespace BibliotecaApp.Application.DTOs;

public class Habilitar2FAResponseDTO
{
    public string Secreto { get; set; } = "";
    public string QrCodeBase64 { get; set; } = "";
}

public class Verificar2FADTO
{
    public string Codigo { get; set; } = "";
}

public class LoginConCodigoDTO
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string? CodigoTwoFactor { get; set; }
}