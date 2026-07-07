using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Domain.Interfaces;

public interface ICategoriaRepository
{
    Categoria Agregar(Categoria categoria);
    Categoria? ObtenerPorId(int id);
    List<Categoria> ObtenerTodos();
    void Actualizar(Categoria categoria);
    void Eliminar(Categoria categoria);
}