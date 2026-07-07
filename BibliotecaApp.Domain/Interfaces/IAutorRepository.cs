using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Domain.Interfaces;
public interface IAutorRepository
{
    Autor Agregar(Autor autor);
    Autor? ObtenerPorId(int id);
    List<Autor> ObtenerTodos();
    void Actualizar(Autor autor);
    void Eliminar(Autor autor);
}