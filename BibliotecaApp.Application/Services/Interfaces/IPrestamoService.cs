using BibliotecaApp.Application.DTOs;

namespace BibliotecaApp.Application.Services.Interfaces;
public interface IPrestamoService
{
    PrestamoResponseDTO Create(PrestamoCreateDTO dto);
    PrestamoResponseDTO Get(int id);
    List<PrestamoResponseDTO> List();
    void Update(int id, PrestamoCreateDTO dto);
    void Delete(int id);
}
