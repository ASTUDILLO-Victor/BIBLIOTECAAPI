using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Domain.Interfaces;

public interface ILibroAutorRepository
{
    void Agregar(LibroAutor libroAutor);
    void EliminarPorLibro(int libroId);
    List<LibroAutor> ObtenerPorLibro(int libroId);
    List<LibroAutor> ObtenerPorAutor(int autorId);
}