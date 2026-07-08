using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;
using BibliotecaApp.Infrastructure.Data;

namespace BibliotecaApp.Infrastructure.Repositories;

public class LibroAutorRepository : ILibroAutorRepository
{
    private readonly AppDbContext _context;

    public LibroAutorRepository(AppDbContext context)
    {
        _context = context;
    }

    public LibroAutor Agregar(LibroAutor libroAutor)
    {
        _context.LibroAutores.Add(libroAutor);
        _context.SaveChanges();
        return libroAutor;
    }

    // public LibroAutor? ObtenerPorId(int id)
    // {
    //     return _context.LibroAutores.Include(la => la.Libro).Include(la => la.Autor).FirstOrDefault(la => la.Id == id);
    // }

    // public List<LibroAutor> ObtenerTodos()
    // {
    //     return _context.LibroAutores.Include(la => la.Libro).Include(la => la.Autor).ToList();
    // }

    public void Eliminar(LibroAutor libroAutor)
    {
        _context.LibroAutores.Remove(libroAutor);
        _context.SaveChanges();
    }

    public List<LibroAutor> ObtenerPorLibro(int libroId)
    {
        return _context.LibroAutores.Include(la => la.Libro).Include(la => la.Autor).Where(la => la.LibroId == libroId).ToList();
    }

    public List<LibroAutor> ObtenerPorAutor(int autorId)
    {
        return _context.LibroAutores.Include(la => la.Libro).Include(la => la.Autor).Where(la => la.AutorId == autorId).ToList();
    }

    public void EliminarPorLibro(int libroId)
    {
        var libroAutores = _context.LibroAutores.Where(la => la.LibroId == libroId).ToList();
        _context.LibroAutores.RemoveRange(libroAutores);
        _context.SaveChanges();
    }
}