using BibliotecaApp.Application.DTOs;

namespace BibliotecaApp.Application.Services.Interfaces;

public interface ILibroService
{
    LibroResponseDTO Agregar(LibroCreateDTO dto);
    LibroResponseDTO ObtenerPorId(int id);
    List<LibroResponseDTO> ObtenerTodos();
    void Actualizar(int id, LibroCreateDTO dto);
    void Eliminar(int id);
}
