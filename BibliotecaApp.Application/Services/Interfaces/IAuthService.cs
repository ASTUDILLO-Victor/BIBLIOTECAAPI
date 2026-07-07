using BibliotecaApp.Application.DTOs;

namespace BibliotecaApp.Application.Services.Interfaces;

public interface IAuthService
{
    AuthResponseDTO Registro(RegistroDTO dto);
    AuthResponseDTO Login(LoginDTO dto);
    void Logout(string accessToken, string refreshToken);
    AuthResponseDTO Refresh(string refreshToken);
    PerfilResponseDTO ObtenerPerfil(int usuarioId);
}