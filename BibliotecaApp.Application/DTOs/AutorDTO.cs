namespace BibliotecaApp.Application.DTOs;

public class AutorCreateDTO
{
    public string Nombre { get; set; } = "";
    public string Nacionalidad { get; set; } = "";
    public DateTime FechaNacimiento { get; set; }
}

public class AutorResponseDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Nacionalidad { get; set; } = "";
    public DateTime FechaNacimiento { get; set; }
    public List<string> Libros { get; set; } = new();
}
