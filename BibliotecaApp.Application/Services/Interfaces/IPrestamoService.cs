using BibliotecaApp.Application.DTOs;

namespace BibliotecaApp.Application.Services.Interfaces;
public interface IPrestamoService
{
    PrestamoResponseDTO Agregar(PrestamoCreateDTO dto);
    PrestamoResponseDTO ObtenerPorId(int id);
    List<PrestamoResponseDTO> ObtenerTodos();
    void Actualizar(int id, PrestamoCreateDTO dto);
    void Eliminar(int id);
}
