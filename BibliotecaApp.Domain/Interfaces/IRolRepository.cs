using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Domain.Interfaces;

public interface IRolRepository
{
    Rol? ObtenerPorNombre(string nombre);
    List<Rol> ObtenerTodos();
}