namespace BibliotecaApp.Domain.Entities
{
    public class Prestamo
    {
        public int Id { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int LibroId { get; set; }
        public Libro? Libro { get; set; }
    }
}