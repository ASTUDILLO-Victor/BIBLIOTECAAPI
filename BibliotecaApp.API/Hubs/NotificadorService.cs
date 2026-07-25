using BibliotecaApp.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BibliotecaApp.API.Hubs;

public class NotificadorService : INotificadorService
{
    private readonly IHubContext<NotificacionesHub> _hubContext;

    public NotificadorService(IHubContext<NotificacionesHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotificarLibroDisponible(int libroId, string tituloLibro)
    {
        await _hubContext.Clients.All.SendAsync("LibroDisponible", new
        {
            libroId,
            tituloLibro,
            mensaje = $"El libro '{tituloLibro}' ya está disponible"
        });
    }
    public async Task NotificarLibroNuevo(int libroId, string tituloLibro)
    {
        await _hubContext.Clients.All.SendAsync("LibroDisponible", new
        {
            libroId,
            tituloLibro,
            mensaje = $"El libro '{tituloLibro}' nuevo"
        });
    }
}