using BibliotecaApp.Application.DTOs;

namespace BibliotecaApp.Application.Services.Interfaces;

public interface ILibroService
{
    LibroResponseDTO Create(LibroCreateDTO dto);
    LibroResponseDTO Get(int id);
    List<LibroResponseDTO> List();
    void Update(int id, LibroCreateDTO dto);
    void Delete(int id);
}
