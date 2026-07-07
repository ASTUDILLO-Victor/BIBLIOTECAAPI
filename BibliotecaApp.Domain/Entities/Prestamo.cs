namespace BibliotecaApp.Domain.Entities
{
 public class Prestamo
{
    public int Id { get; set; }
    public DateTime FechaPrestamo { get; set; } = DateTime.UtcNow;
    public DateTime? FechaDevolucion { get; set; }  // nullable — aún no devuelto
    public string Estado { get; set; } = "Activo";  // Activo, Devuelto, Vencido
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public int LibroId { get; set; }
    public Libro? Libro { get; set; }
}
}