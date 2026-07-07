using BibliotecaApp.Application.DTOs;

namespace BibliotecaApp.Application.Services.Interfaces;

public interface IAutorService
{
    AutorResponseDTO Agregar(AutorCreateDTO dto);
    AutorResponseDTO? ObtenerPorId(int id);
    List<AutorResponseDTO> ObtenerTodos();
    AutorResponseDTO? Actualizar(int id, AutorCreateDTO dto);
    bool Eliminar(int id);
}
