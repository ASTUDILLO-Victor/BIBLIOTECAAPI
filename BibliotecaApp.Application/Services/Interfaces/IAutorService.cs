using BibliotecaApp.Application.DTOs;

namespace BibliotecaApp.Application.Services.Interfaces;

public interface IAutorService
{
    AutorResponseDTO Create(AutorCreateDTO dto);
    AutorResponseDTO Get(int id);
    List<AutorResponseDTO> List();
    void Update(int id, AutorCreateDTO dto);
    void Delete(int id);
}
