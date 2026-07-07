using BibliotecaApp.Application.DTOs;

namespace BibliotecaApp.Application.Services.Interfaces;

public interface ICategoriaService
{
    CategoriaResponseDTO Agregar(CategoriaCreateDTO dto);
    CategoriaResponseDTO ObtenerPorId(int id);
    List<CategoriaResponseDTO> ObtenerTodos();
    void Actualizar(int id, CategoriaCreateDTO dto);
    void Eliminar(int id);
}
