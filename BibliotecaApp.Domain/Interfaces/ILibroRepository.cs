using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Domain.Interfaces;

public interface ILibroRepository
{
    Libro Agregar(Libro libro);
    Libro? ObtenerPorId(int id);
    List<Libro> ObtenerTodos();
    List<Libro> ObtenerPorCategoria(int categoriaId);
    List<Libro> ObtenerPorAutor(int autorId);
    void Actualizar(Libro libro);
    void Eliminar(Libro libro);
}