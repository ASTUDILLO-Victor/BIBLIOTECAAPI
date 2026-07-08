using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;
using BibliotecaApp.Infrastructure.Data;

namespace BibliotecaApp.Infrastructure.Repositories;

public class AutorRepository : IAutorRepository
{
    private readonly AppDbContext _context;

    public AutorRepository(AppDbContext context)
    {
        _context = context;
    }

    public Autor Agregar(Autor autor)
    {
        _context.Autores.Add(autor);
        _context.SaveChanges();
        return autor;
    }

    public Autor? ObtenerPorId(int id)
    {
        return _context.Autores
            .Include(a => a.LibroAutores)
                .ThenInclude(la => la.Libro)
            .FirstOrDefault(a => a.Id == id);
    }

    public List<Autor> ObtenerTodos()
    {
        return _context.Autores
            .Include(a => a.LibroAutores)
                .ThenInclude(la => la.Libro)
            .AsNoTracking()
            .ToList();
    }

    public void Actualizar(Autor autor)
    {
        _context.Autores.Update(autor);
        _context.SaveChanges();
    }

    public void Eliminar(Autor autor)
    {
        _context.Autores.Remove(autor);
        _context.SaveChanges();
    }
}