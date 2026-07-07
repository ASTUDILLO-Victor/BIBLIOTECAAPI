using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Domain.Interfaces;

public interface ILibroRepository
{
    Libro Agregar(Libro libro);
    Libro? ObtenerPorId(int id);
    List<Libro> ObtenerTodos();
    void Actualizar(Libro libro);
    void Eliminar(Libro libro);
}