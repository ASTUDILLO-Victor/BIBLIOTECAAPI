namespace BibliotecaApp.Domain.Entities
{
    public class Autor
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Nacionalidad { get; set; } = "";
        public DateTime FechaNacimiento { get; set; }

        public List<LibroAutor> LibroAutores { get; set; } = new();
    }
}