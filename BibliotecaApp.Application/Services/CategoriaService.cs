using BibliotecaApp.Application.DTOs;
using BibliotecaApp.Application.Services.Interfaces;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;

namespace BibliotecaApp.Application.Services;
public class CategoriaService : ICategoriaService
{
     private readonly ICategoriaRepository _categoriaRepository;
     public CategoriaService(
        ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public CategoriaResponseDTO Agregar(CategoriaCreateDTO dto)
    {
        var categoria = new Categoria
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        _categoriaRepository.Agregar(categoria);

        return MapearCategoria(categoria);
    }

    public CategoriaResponseDTO ObtenerPorId(int id)
    {
        var categoria = _categoriaRepository.ObtenerPorId(id);
        if (categoria == null)
        {
            throw new Exception("Categoría no encontrada");
        }

        return MapearCategoria(categoria);
    }

    public List<CategoriaResponseDTO> ObtenerTodos()
    {
        var categorias = _categoriaRepository.ObtenerTodos();
        return categorias.Select(MapearCategoria).ToList();
       
    }

    public CategoriaResponseDTO? Actualizar (int id, CategoriaCreateDTO dto) 
    {
        var categoria = _categoriaRepository.ObtenerPorId(id);
        if (categoria == null)
        {
            throw new Exception("Categoría no encontrada");
        }

        categoria.Nombre = dto.Nombre;
        categoria.Descripcion = dto.Descripcion;

        _categoriaRepository.Actualizar(categoria);

        return MapearCategoria(categoria);
    }

    public  bool Eliminar(int id)
    {
        var categoria = _categoriaRepository.ObtenerPorId(id);
        if (categoria == null)
        {
            throw new Exception("Categoría no encontrada");
        }

        _categoriaRepository.Eliminar(categoria);
        return true;
    }


    private CategoriaResponseDTO MapearCategoria(Categoria categoria)
    {
        return new CategoriaResponseDTO
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            TotalLibros = categoria.Libros.Count
        };
    }
}