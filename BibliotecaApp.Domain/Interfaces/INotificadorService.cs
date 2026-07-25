using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Interfaces;

public interface INotificadorService
{
    Task NotificarLibroDisponible(int libroId, string tituloLibro);
    Task NotificarLibroNuevo(int libroId, string tituloLibro);
}