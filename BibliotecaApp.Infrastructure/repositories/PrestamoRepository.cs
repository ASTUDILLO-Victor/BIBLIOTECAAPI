using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;
using BibliotecaApp.Infrastructure.Data;

namespace BibliotecaApp.Infrastructure.Repositories;

public class PrestamoRepository : IPrestamoRepository
{
    private readonly AppDbContext _context;

    public PrestamoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Prestamo Agregar(Prestamo prestamo)
    {
        _context.Prestamos.Add(prestamo);
        _context.SaveChanges();
        return prestamo;
    }

    public Prestamo? ObtenerPorId(int id)
    {
        return _context.Prestamos.Include(p => p.Libro).Include(p => p.Usuario).FirstOrDefault(p => p.Id == id);
    }

    public List<Prestamo> ObtenerTodos()
    {
        return _context.Prestamos.Include(p => p.Libro).Include(p => p.Usuario).ToList();
    }

    public List<Prestamo> ObtenerPorUsuario(int usuarioId)
    {
        return _context.Prestamos.Include(p => p.Libro).Include(p => p.Usuario).Where(p => p.UsuarioId == usuarioId).ToList();
    }

    public int ContarPrestamosActivos(int usuarioId)
    {
        return _context.Prestamos.Count(p => p.UsuarioId == usuarioId && p.FechaDevolucion == null);
    }

    public void Actualizar(Prestamo prestamo)
    {
        _context.Prestamos.Update(prestamo);
        _context.SaveChanges();
    }
}