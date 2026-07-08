using BibliotecaApp.Application.DTOs;

namespace BibliotecaApp.Application.Services.Interfaces;

public interface ILibroService
{
    LibroResponseDTO Agregar(LibroCreateDTO dto);
    LibroResponseDTO ObtenerPorId(int id);
    List<LibroResponseDTO> ObtenerTodos();
    LibroResponseDTO? Actualizar(int id, LibroCreateDTO dto);
    bool Eliminar(int id);
}
