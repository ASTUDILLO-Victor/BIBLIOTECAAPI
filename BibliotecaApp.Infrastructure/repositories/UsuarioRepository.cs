using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;
using BibliotecaApp.Infrastructure.Data;

namespace BibliotecaApp.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<Usuario> QueryBase()
    {
        return _context.Usuarios
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
                    .ThenInclude(r => r!.RolPermisos)
                        .ThenInclude(rp => rp.Permiso)
            .Include(u => u.RefreshTokens);
    }

    public Usuario? ObtenerPorId(int id)
    {
        return QueryBase().FirstOrDefault(u => u.Id == id);
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        return QueryBase().FirstOrDefault(u => u.Email == email);
    }

    public bool ExisteEmail(string email)
    {
        return _context.Usuarios.Any(u => u.Email == email);
    }

    public Usuario Agregar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
        return usuario;
    }

    public Usuario Actualizar(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        _context.SaveChanges();
        return usuario;
    }

    public List<Usuario> ObtenerTodos()
    {
        return QueryBase().AsNoTracking().ToList();
    }
}