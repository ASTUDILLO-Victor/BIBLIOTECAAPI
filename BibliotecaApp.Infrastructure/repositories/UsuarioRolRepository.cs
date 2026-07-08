using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;
using BibliotecaApp.Infrastructure.Data;

namespace BibliotecaApp.Infrastructure.Repositories;

public class UsuarioRolRepository : IUsuarioRolRepository
{
    private readonly AppDbContext _context;

    public UsuarioRolRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Agregar(UsuarioRol usuarioRol)
    {
        _context.UsuarioRoles.Add(usuarioRol);
        _context.SaveChanges();
    }

    public List<UsuarioRol> ObtenerPorUsuario(int usuarioId)
    {
        return _context.UsuarioRoles
            .Include(ur => ur.Rol)
                .ThenInclude(r => r!.RolPermisos)
                    .ThenInclude(rp => rp.Permiso)
            .Where(ur => ur.UsuarioId == usuarioId)
            .ToList();
    }
}