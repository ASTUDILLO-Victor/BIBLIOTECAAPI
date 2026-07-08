using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;
using BibliotecaApp.Infrastructure.Data;

namespace BibliotecaApp.Infrastructure.Repositories;

public class TokenBlacklistRepository : ITokenBlacklistRepository
{
    private readonly AppDbContext _context;

    public TokenBlacklistRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Agregar(TokenBlacklist token)
    {
        _context.TokenBlacklist.Add(token);
        _context.SaveChanges();
    }

    public bool EstaEnBlacklist(string token)
    {
        return _context.TokenBlacklist.Any(t => t.Token == token);
    }

    public void EliminarExpirados()
    {
        _context.Database.ExecuteSqlRaw(
            "DELETE FROM \"TokenBlacklist\" WHERE \"FechaExpiracion\" < {0}",
            DateTime.UtcNow);
    }
}