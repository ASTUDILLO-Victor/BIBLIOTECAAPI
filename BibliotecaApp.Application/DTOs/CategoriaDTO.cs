namespace BibliotecaApp.Application.DTOs;

public class CategoriaCreateDTO
{
    public string Nombre { get; set; } = "";
}

public class CategoriaResponseDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int TotalProductos { get; set; }
}