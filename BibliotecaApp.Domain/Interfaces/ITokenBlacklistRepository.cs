using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Domain.Interfaces;

public interface ITokenBlacklistRepository
{
    void Agregar(TokenBlacklist token);
    bool EstaEnBlacklist(string token);
    void EliminarExpirados();
}