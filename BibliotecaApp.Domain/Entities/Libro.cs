namespace BibliotecaApp.Domain.Entities
{
    public class Libro
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string ISBN { get; set; } = "";
    public int AnioPublicacion { get; set; }
    public int CopiasDisponibles { get; set; }  // ← para validar préstamos
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public List<LibroAutor> LibroAutores { get; set; } = new();
    public List<Prestamo> Prestamos { get; set; } = new();
}
}
    
