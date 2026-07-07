using BibliotecaApp.Application.DTOs;

namespace BibliotecaApp.Application.Services.Interfaces;

public interface ICategoriaService
{
    CategoriaResponseDTO Create(CategoriaCreateDTO dto);
    CategoriaResponseDTO Get(int id);
    List<CategoriaResponseDTO> List();
    void Update(int id, CategoriaCreateDTO dto);
    void Delete(int id);
}
