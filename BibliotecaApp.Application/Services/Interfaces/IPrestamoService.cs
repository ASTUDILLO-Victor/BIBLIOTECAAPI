using BibliotecaApp.Application.DTOs;

namespace BibliotecaApp.Application.Services.Interfaces;

public interface IPrestamoService
{
    PrestamoResponseDTO Agregar(PrestamoCreateDTO dto, int usuarioId);
    PrestamoResponseDTO? ObtenerPorId(int id);
    List<PrestamoResponseDTO> ObtenerTodos();
    List<PrestamoResponseDTO> ObtenerPorUsuario(int usuarioId);
    PrestamoResponseDTO? Devolver(int id);
}