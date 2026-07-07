namespace BibliotecaApp.Application.DTOs;


    public class PrestamoCreateDTO
    {
        public int LibroId { get; set; }  // ← solo esto
    }

    public class PrestamoResponseDTO
    {
        public int Id { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public string NombreUsuario { get; set; }
        public string TituloLibro { get; set; }
    }
