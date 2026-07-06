using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    RefreshToken Agregar(RefreshToken refreshToken);
    RefreshToken? ObtenerPorToken(string token);
    void Revocar(RefreshToken refreshToken);
    void EliminarExpirados();
}