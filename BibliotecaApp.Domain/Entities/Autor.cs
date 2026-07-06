namespace BibliotecaApp.Domain.Entities
{
    public class Autor
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Nacionalidade { get; set; }
        public DateTime DataNascimento { get; set; }

        public List<LibroAutor> LibroAutores { get; set; } = new();
    }
}