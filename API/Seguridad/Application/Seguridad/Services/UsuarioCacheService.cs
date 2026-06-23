using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.Domain.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class UsuarioCacheService
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private const string UsuariosCacheKey = "UsuariosCacheKey";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public UsuarioCacheService(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public Dictionary<string, Usuario> GetUsuarios()
    {
        if (!_cache.TryGetValue(UsuariosCacheKey, out Dictionary<string, Usuario> usuarios))
        {
            usuarios = _context
                .Usuarios
                .AsNoTracking()
                .ToDictionary(u => u.Id);
            
            _cache.Set(UsuariosCacheKey, usuarios, CacheDuration);
        }
        return usuarios;
    }

    public Usuario? GetUsuarioById(string id)
    {
        if (id.IsNullOrWhiteSpace())
            return null;

        var usuarios = GetUsuarios();
        usuarios.TryGetValue(id, out var usuario);
        return usuario;
    }

    public string GetUsuarioNombreCompletoById(string id)
    {
        var usuario = GetUsuarioById(id);
        return usuario != null ? usuario.NombreCompleto : "Usuario Desconocido";
    }

    public void ClearCache()
    {
        if (_cache is null)
            return;

        _cache.Remove(UsuariosCacheKey);
    }
}