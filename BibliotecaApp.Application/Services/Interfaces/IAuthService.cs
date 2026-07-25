using BibliotecaApp.Application.DTOs;

namespace BibliotecaApp.Application.Services.Interfaces;

public interface IAuthService
{
    AuthResponseDTO Registro(RegistroDTO dto);
    AuthResponseDTO Login(LoginConCodigoDTO dto);
    void Logout(string accessToken, string refreshToken);
    AuthResponseDTO Refresh(string refreshToken);
    PerfilResponseDTO ObtenerPerfil(int usuarioId);

    // Nuevos métodos para 2FA
    Habilitar2FAResponseDTO Habilitar2FA(int usuarioId);
    void ConfirmarHabilitacion2FA(int usuarioId, Verificar2FADTO dto);
    AuthResponseDTO LoginCon2FA(LoginConCodigoDTO dto);
    void Deshabilitar2FA(int usuarioId);
    AuthResponseDTO LoginConGoogle(string email, string nombre);
}