using System;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Interfaces;

public interface ICacheService
{
    Task<T?> ObtenerAsync<T>(string key);
    Task GuardarAsync<T>(string key, T valor, TimeSpan? expiracion = null);
    Task EliminarAsync(string key);
}