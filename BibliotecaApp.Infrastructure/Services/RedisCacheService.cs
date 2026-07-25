using BibliotecaApp.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace BibliotecaApp.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> ObtenerAsync<T>(string key)
    {
        var valorJson = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(valorJson))
            return default;

        return JsonSerializer.Deserialize<T>(valorJson);
    }

    public async Task GuardarAsync<T>(string key, T valor, TimeSpan? expiracion = null)
    {
        var valorJson = JsonSerializer.Serialize(valor);

        var opciones = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiracion ?? TimeSpan.FromMinutes(5)
        };

        await _cache.SetStringAsync(key, valorJson, opciones);
    }

    public async Task EliminarAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}