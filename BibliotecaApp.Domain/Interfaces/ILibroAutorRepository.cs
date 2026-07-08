using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Domain.Interfaces;

public interface ILibroAutorRepository
{
    LibroAutor Agregar(LibroAutor libroAutor);

    void Eliminar(LibroAutor libroAutor);

    void EliminarPorLibro(int libroId);
    List<LibroAutor> ObtenerPorLibro(int libroId);
    List<LibroAutor> ObtenerPorAutor(int autorId);
}