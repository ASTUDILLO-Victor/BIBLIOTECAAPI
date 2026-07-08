using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;
using BibliotecaApp.Infrastructure.Data;

namespace BibliotecaApp.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public RefreshToken Agregar(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        _context.SaveChanges();
        return refreshToken;
    }

    public RefreshToken? ObtenerPorToken(string token)
    {
        return _context.RefreshTokens
            .FirstOrDefault(rt => rt.Token == token);
    }

    public void Revocar(RefreshToken refreshToken)
    {
        refreshToken.EstaRevocado = true;
        _context.SaveChanges();
    }

    public void EliminarExpirados()
    {
        _context.Database.ExecuteSqlRaw(
            "DELETE FROM \"RefreshTokens\" WHERE \"FechaExpiracion\" < {0}",
            DateTime.UtcNow);
    }
}