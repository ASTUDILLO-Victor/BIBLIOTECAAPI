namespace BibliotecaApp.Domain.Entities
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string ISBN { get; set; }
        public DateTime DataPublicacao { get; set; }
        public List<LibroAutor> LibroAutores { get; set; } = new();

        public List<Prestamo> Prestamos { get; set; } = new();
    }
}
    
