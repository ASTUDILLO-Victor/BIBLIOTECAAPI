using BibliotecaApp.Application.DTOs;
using BibliotecaApp.Application.Services.Interfaces;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;

namespace BibliotecaApp.Application.Services;

public class AutorService : IAutorService
{
     private readonly IAutorRepository _autorRepository;
     public AutorService(
        IAutorRepository autorRepository)
    {
        _autorRepository = autorRepository;
    }
    public AutorResponseDTO Agregar(AutorCreateDTO dto)
    {
        var autor = new Autor
        {
            Nombre = dto.Nombre,
            Nacionalidad = dto.Nacionalidad,
            FechaNacimiento = dto.FechaNacimiento
        };

        var creado = _autorRepository.Agregar(autor);

        return new AutorResponseDTO
        {
            Id = creado.Id,
            Nombre = creado.Nombre,
            FechaNacimiento = creado.FechaNacimiento,
            Nacionalidad = creado.Nacionalidad
        };
    }

    public AutorResponseDTO? ObtenerPorId(int id)
    {
        var autor = _autorRepository.ObtenerPorId(id);
        if (autor == null)
            return null;

        return new AutorResponseDTO
        {
            Id = autor.Id,
            Nombre = autor.Nombre,
            FechaNacimiento = autor.FechaNacimiento,
            Nacionalidad = autor.Nacionalidad
        };
    }

    public List<AutorResponseDTO> ObtenerTodos()
    {
        var autores = _autorRepository.ObtenerTodos();
        return autores.Select(autor => new AutorResponseDTO
        {
            Id = autor.Id,
            Nombre = autor.Nombre,
            FechaNacimiento = autor.FechaNacimiento,
            Nacionalidad = autor.Nacionalidad
        }).ToList();
    }

    public AutorResponseDTO? Actualizar(int id, AutorCreateDTO dto)
    {
        var autor = _autorRepository.ObtenerPorId(id);
        if (autor == null) return null;

        autor.Nombre = dto.Nombre;
        autor.Nacionalidad = dto.Nacionalidad;
        autor.FechaNacimiento = dto.FechaNacimiento;
        _autorRepository.Actualizar(autor);

        return MapearAutor(autor);
    }

    public bool Eliminar(int id)
    {
        var autor = _autorRepository.ObtenerPorId(id);
        if (autor == null) return false;

        if (autor.LibroAutores.Count > 0)
            throw new ArgumentException("No se puede eliminar un autor que tiene libros");

        _autorRepository.Eliminar(autor);
        return true;
    }

    private AutorResponseDTO MapearAutor(Autor autor)
    {
        return new AutorResponseDTO
        {
            Id = autor.Id,
            Nombre = autor.Nombre,
            Nacionalidad = autor.Nacionalidad,
            FechaNacimiento = autor.FechaNacimiento,
            Libros = autor.LibroAutores
                .Select(la => la.Libro?.Titulo ?? "")
                .ToList()
        };
    }
}