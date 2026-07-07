using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Domain.Interfaces;
public interface IPrestamoRepository
{
    Prestamo Agregar(Prestamo prestamo);
    Prestamo? ObtenerPorId(int id);
    List<Prestamo> ObtenerTodos();
    List<Prestamo> ObtenerPorUsuario(int usuarioId);
    int ContarPrestamosActivos(int usuarioId);  // ← para validar máximo 3
    void Actualizar(Prestamo prestamo);
}