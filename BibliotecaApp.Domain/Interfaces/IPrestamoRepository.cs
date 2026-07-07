using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Domain.Interfaces;
public interface IPrestamoRepository
{
    Prestamo Agregar(Prestamo prestamo);
    Prestamo? ObtenerPorId(int id);
    List<Prestamo> ObtenerTodos();
    void Actualizar(Prestamo prestamo);
    void Eliminar(Prestamo prestamo);
}