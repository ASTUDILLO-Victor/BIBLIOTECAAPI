namespace BibliotecaApp.Application.DTOs;

public class AutorCreateDTO
{
    public string Nome { get; set; }
    public string Nacionalidade { get; set; }
    public DateTime DataNascimento { get; set; }
}

public class AutorResponseDTO
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Nacionalidade { get; set; }
    public DateTime DataNascimento { get; set; }
}
