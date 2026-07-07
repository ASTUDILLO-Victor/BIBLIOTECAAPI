namespace BibliotecaApp.Application.DTOs;

public class LibroCreateDTO
{
    public string Titulo { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string ISBN { get; set; } = "";
    public int AnioPublicacion { get; set; }
    public int CopiasDisponibles { get; set; }
    public int CategoriaId { get; set; }
    public List<int> AutorIds { get; set; } = new();  // ← relación muchos a muchos
}

public class LibroResponseDTO
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string ISBN { get; set; } = "";
    public DateTime DataPublicacao { get; set; }

    public int TotalPrestamos { get; set; }
    public int TotalAutores { get; set; }
}