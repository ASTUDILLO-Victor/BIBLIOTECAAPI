using BibliotecaApp.Application.DTOs;
using BibliotecaApp.Application.Services.Interfaces;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;

namespace BibliotecaApp.Application.Services;

public class PrestamoService : IPrestamoService
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly ILibroRepository _libroRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public PrestamoService(
        IPrestamoRepository prestamoRepository,
        ILibroRepository libroRepository,
        IUsuarioRepository usuarioRepository)
    {
        _prestamoRepository = prestamoRepository;
        _libroRepository = libroRepository;
        _usuarioRepository = usuarioRepository;
    }

    public PrestamoResponseDTO Agregar(PrestamoCreateDTO dto, int usuarioId)
    {
        var libro = _libroRepository.ObtenerPorId(dto.LibroId);
        if (libro == null)
            throw new KeyNotFoundException("Libro no encontrado");

        if (libro.CopiasDisponibles <= 0)
            throw new ArgumentException("No hay copias disponibles para este libro");

        int prestamosActivos = _prestamoRepository.ContarPrestamosActivos(usuarioId);
        if (prestamosActivos >= 3)
            throw new ArgumentException("No puedes tener más de 3 préstamos activos");

        var prestamo = new Prestamo
        {
            LibroId = dto.LibroId,
            UsuarioId = usuarioId,
            FechaPrestamo = DateTime.UtcNow
        };

        _prestamoRepository.Agregar(prestamo);

        libro.CopiasDisponibles--;
        _libroRepository.Actualizar(libro);

        return MapearPrestamo(_prestamoRepository.ObtenerPorId(prestamo.Id)!);
    }

    public PrestamoResponseDTO? ObtenerPorId(int id)
    {
        var prestamo = _prestamoRepository.ObtenerPorId(id);
        if (prestamo == null) return null;
        return MapearPrestamo(prestamo);
    }

    public List<PrestamoResponseDTO> ObtenerTodos()
    {
        return _prestamoRepository.ObtenerTodos()
            .Select(p => MapearPrestamo(p))
            .ToList();
    }

    public List<PrestamoResponseDTO> ObtenerPorUsuario(int usuarioId)
    {
        return _prestamoRepository.ObtenerPorUsuario(usuarioId)
            .Select(p => MapearPrestamo(p))
            .ToList();
    }

    public PrestamoResponseDTO? Devolver(int id)
    {
        var prestamo = _prestamoRepository.ObtenerPorId(id);
        if (prestamo == null) return null;

        if (prestamo.Estado == "Devuelto")
            throw new ArgumentException("Este préstamo ya fue devuelto");

        prestamo.Estado = "Devuelto";
        prestamo.FechaDevolucion = DateTime.UtcNow;
        _prestamoRepository.Actualizar(prestamo);

        var libro = _libroRepository.ObtenerPorId(prestamo.LibroId);
        if (libro != null)
        {
            libro.CopiasDisponibles++;
            _libroRepository.Actualizar(libro);
        }

        return MapearPrestamo(_prestamoRepository.ObtenerPorId(prestamo.Id)!);
    }

    private PrestamoResponseDTO MapearPrestamo(Prestamo prestamo)
    {
        return new PrestamoResponseDTO
        {
            Id = prestamo.Id,
            FechaPrestamo = prestamo.FechaPrestamo,
            FechaDevolucion = prestamo.FechaDevolucion,
            Estado = prestamo.Estado,
            TituloLibro = prestamo.Libro?.Titulo ?? "",
            NombreUsuario = prestamo.Usuario?.Nombre ?? ""
        };
    }
}