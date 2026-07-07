namespace BibliotecaApp.Application.DTOs;

public class LibroCreateDTO
{
    public string Titulo { get; set; }
    public string ISBN { get; set; }
    public DateTime DataPublicacao { get; set; }
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