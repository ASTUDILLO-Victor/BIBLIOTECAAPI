using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;
using BibliotecaApp.Infrastructure.Data;

namespace BibliotecaApp.Infrastructure.Repositories;

public class LibroRepository : ILibroRepository
{
    private readonly AppDbContext _context;

    public LibroRepository(AppDbContext context)
    {
        _context = context;
    }

    public Libro Agregar(Libro libro)
    {
        _context.Libros.Add(libro);
        _context.SaveChanges();
        return libro;
    }

    public Libro? ObtenerPorId(int id)
    {
        return _context.Libros.Include(l => l.Categoria).Include(l => l.LibroAutores).ThenInclude(la => la.Autor).FirstOrDefault(l => l.Id == id);
    }

    public List<Libro> ObtenerTodos()
    {
        return _context.Libros.Include(l => l.Categoria).Include(l => l.LibroAutores).ThenInclude(la => la.Autor).AsNoTracking().ToList();
    }

    public List<Libro> ObtenerPorCategoria(int categoriaId)
    {
        return _context.Libros.Include(l => l.Categoria).Include(l => l.LibroAutores).ThenInclude(la => la.Autor).Where(l => l.CategoriaId == categoriaId).ToList();
    }

    public List<Libro> ObtenerPorAutor(int autorId)
    {
        return _context.Libros.Include(l => l.Categoria).Include(l => l.LibroAutores).ThenInclude(la => la.Autor).Where(l => l.LibroAutores.Any(la => la.AutorId == autorId)).ToList();
    }

    public void Actualizar(Libro libro)
    {
        _context.Libros.Update(libro);
        _context.SaveChanges();
    }

    public void Eliminar(Libro libro)
    {
        _context.Libros.Remove(libro);
        _context.SaveChanges();
    }
}