using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;
using BibliotecaApp.Infrastructure.Data;

namespace BibliotecaApp.Infrastructure.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;

    public CategoriaRepository(AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<Categoria> QueryBase()
    {
        return _context.Categorias.Include(c => c.Libros);
    }

    public List<Categoria> ObtenerTodos()
    {
        return QueryBase().AsNoTracking().ToList();
    }

    public Categoria? ObtenerPorId(int id)
    {
        return QueryBase().FirstOrDefault(c => c.Id == id);
    }

    public Categoria Agregar(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        _context.SaveChanges();
        return categoria;
    }

    public Categoria Actualizar(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
        _context.SaveChanges();
        return categoria;
    }

    public void Eliminar(Categoria categoria)
    {
        _context.Categorias.Remove(categoria);
        _context.SaveChanges();
    }
}