namespace BibliotecaApp.Application.DTOs;

public class CategoriaCreateDTO
{
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
}

public class CategoriaResponseDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int TotalLibros { get; set; }
}