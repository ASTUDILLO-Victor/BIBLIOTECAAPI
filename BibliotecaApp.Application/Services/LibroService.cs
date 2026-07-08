using BibliotecaApp.Application.DTOs;
using BibliotecaApp.Application.Services.Interfaces;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;

namespace BibliotecaApp.Application.Services;
public class LibroService : ILibroService
{
     private readonly ILibroRepository _libroRepository;
     private readonly IAutorRepository _autorRepository;
     private readonly ILibroAutorRepository _libroAutorRepository;
     public LibroService(
        ILibroRepository libroRepository,
        IAutorRepository autorRepository,
        ILibroAutorRepository libroAutorRepository)
    {
        _libroRepository = libroRepository;
        _autorRepository = autorRepository;
        _libroAutorRepository = libroAutorRepository;
    }

    public LibroResponseDTO Agregar(LibroCreateDTO dto)
    {
        var libro = new Libro
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            ISBN = dto.ISBN,
            AnioPublicacion = dto.AnioPublicacion,
            CopiasDisponibles = dto.CopiasDisponibles,
            CategoriaId = dto.CategoriaId
        };

        _libroRepository.Agregar(libro);

        foreach (var autorId in dto.AutorIds)
        {
            var autor = _autorRepository.ObtenerPorId(autorId);
            if (autor != null)
            {
                var libroAutor = new LibroAutor
                {
                    LibroId = libro.Id,
                    AutorId = autor.Id
                };
                _libroAutorRepository.Agregar(libroAutor);
            }
        }

        return MapearLibro(libro);
    }

    public LibroResponseDTO ObtenerPorId(int id)
    {
        var libro = _libroRepository.ObtenerPorId(id);
        if (libro == null)
        {
            throw new Exception("Libro no encontrado");
        }
        return MapearLibro(libro);
    }

    public List<LibroResponseDTO> ObtenerTodos()
    {
        var libros = _libroRepository.ObtenerTodos();
        return libros.Select(MapearLibro).ToList();
    }

    public LibroResponseDTO? Actualizar(int id, LibroCreateDTO dto)
    {
        var libro = _libroRepository.ObtenerPorId(id);
        if (libro == null)
        {
            throw new Exception("Libro no encontrado");
        }

        libro.Titulo = dto.Titulo;
        libro.Descripcion = dto.Descripcion;
        libro.ISBN = dto.ISBN;
        libro.AnioPublicacion = dto.AnioPublicacion;
        libro.CopiasDisponibles = dto.CopiasDisponibles;
        libro.CategoriaId = dto.CategoriaId;

        _libroRepository.Actualizar(libro);

        // Actualizar autores
        var autoresExistentes = libro.LibroAutores.Select(la => la.AutorId).ToList();
        var autoresNuevos = dto.AutorIds.Except(autoresExistentes).ToList();
        var autoresEliminados = autoresExistentes.Except(dto.AutorIds).ToList();

        foreach (var autorId in autoresNuevos)
        {
            var autor = _autorRepository.ObtenerPorId(autorId);
            if (autor != null)
            {
                var libroAutor = new LibroAutor
                {
                    LibroId = libro.Id,
                    AutorId = autor.Id
                };
                _libroAutorRepository.Agregar(libroAutor);
            }
        }

        foreach (var autorId in autoresEliminados)
        {
            var libroAutor = libro.LibroAutores.FirstOrDefault(la => la.AutorId == autorId);
            if (libroAutor != null)
            {
                _libroAutorRepository.Eliminar(libroAutor);
            }
        }
        return MapearLibro(libro);
    }

    public bool Eliminar(int id)
    {
        var libro = _libroRepository.ObtenerPorId(id);
        if (libro == null)
        {
            throw new Exception("Libro no encontrado");
        }

        // Eliminar relaciones con autores
        _libroAutorRepository.EliminarPorLibro(libro.Id);

        _libroRepository.Eliminar(libro);

        return true;
    }

    private LibroResponseDTO MapearLibro(Libro libro)
    {
        return new LibroResponseDTO
        {
            Id = libro.Id,
            Titulo = libro.Titulo,
            Descripcion = libro.Descripcion,
            ISBN = libro.ISBN,
            AnioPublicacion = libro.AnioPublicacion,
            CopiasDisponibles = libro.CopiasDisponibles,
            CategoriaNombre = libro.Categoria?.Nombre ?? "Sin categoría",
            Autores = libro.LibroAutores.Select(la => la.Autor?.Nombre ?? "").ToList(),
            TotalPrestamos = libro.Prestamos.Count
        };
    }
}