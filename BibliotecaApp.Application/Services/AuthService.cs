using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BibliotecaApp.Application.DTOs;
using BibliotecaApp.Application.Services.Interfaces;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces;

namespace BibliotecaApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenBlacklistRepository _blacklistRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IUsuarioRolRepository _usuarioRolRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenBlacklistRepository blacklistRepository,
        IRolRepository rolRepository,
        IUsuarioRolRepository usuarioRolRepository,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _blacklistRepository = blacklistRepository;
        _rolRepository = rolRepository;
        _usuarioRolRepository = usuarioRolRepository;
        _configuration = configuration;
    }

    public AuthResponseDTO Registro(RegistroDTO dto)
    {
        if (dto.Password != dto.ConfirmarPassword)
            throw new ArgumentException("Las contraseñas no coinciden");

        if (_usuarioRepository.ExisteEmail(dto.Email))
            throw new ArgumentException("El email ya está registrado");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email,
            Telefono = dto.Telefono,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Activo = true,
            FechaRegistro = DateTime.UtcNow
        };

        var creado = _usuarioRepository.Agregar(usuario);

        var rolCliente = _rolRepository.ObtenerPorNombre("Cliente");
        if (rolCliente == null)
            throw new KeyNotFoundException("Rol Cliente no encontrado en BD");

        _usuarioRolRepository.Agregar(new UsuarioRol
        {
            UsuarioId = creado.Id,
            RolId = rolCliente.Id
        });

        var usuarioConRoles = _usuarioRepository.ObtenerPorId(creado.Id)!;
        return GenerarAuthResponse(usuarioConRoles);
    }

    public AuthResponseDTO Login(LoginDTO dto)
    {
        var usuario = _usuarioRepository.ObtenerPorEmail(dto.Email);
        if (usuario == null)
            throw new UnauthorizedAccessException("Email o contraseña incorrectos");

        if (!usuario.Activo)
            throw new UnauthorizedAccessException("Usuario desactivado. Contacte al administrador");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            throw new UnauthorizedAccessException("Email o contraseña incorrectos");

        usuario.UltimoLogin = DateTime.UtcNow;
        _usuarioRepository.Actualizar(usuario);

        return GenerarAuthResponse(usuario);
    }

    public void Logout(string accessToken, string refreshToken)
    {
        var expiracion = ObtenerExpiracionToken(accessToken);
        _blacklistRepository.Agregar(new TokenBlacklist
        {
            Token = accessToken,
            FechaExpiracion = expiracion
        });

        var rt = _refreshTokenRepository.ObtenerPorToken(refreshToken);
        if (rt != null)
            _refreshTokenRepository.Revocar(rt);
    }

    public AuthResponseDTO Refresh(string refreshToken)
    {
        var rt = _refreshTokenRepository.ObtenerPorToken(refreshToken);

        if (rt == null)
            throw new ArgumentException("Refresh token inválido");

        if (rt.EstaRevocado)
            throw new ArgumentException("Refresh token revocado");

        if (rt.FechaExpiracion < DateTime.UtcNow)
            throw new ArgumentException("Refresh token expirado");

        var usuario = _usuarioRepository.ObtenerPorId(rt.UsuarioId);
        if (usuario == null)
            throw new KeyNotFoundException("Usuario no encontrado");

        _refreshTokenRepository.Revocar(rt);
        return GenerarAuthResponse(usuario);
    }

    public PerfilResponseDTO ObtenerPerfil(int usuarioId)
    {
        var usuario = _usuarioRepository.ObtenerPorId(usuarioId);
        if (usuario == null)
            throw new KeyNotFoundException("Usuario no encontrado");

        return MapearPerfil(usuario);
    }

    private AuthResponseDTO GenerarAuthResponse(Usuario usuario)
    {
        string accessToken = GenerarAccessToken(usuario);
        string refreshToken = GenerarRefreshToken();

        int refreshDays = int.Parse(_configuration["Jwt:RefreshTokenDays"]!);

        _refreshTokenRepository.Agregar(new RefreshToken
        {
            Token = refreshToken,
            UsuarioId = usuario.Id,
            FechaExpiracion = DateTime.UtcNow.AddDays(refreshDays)
        });

        return new AuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.UsuarioRoles.FirstOrDefault()?.Rol?.Nombre ?? "",
            UltimoLogin = usuario.UltimoLogin
        };
    }

    private string GenerarAccessToken(Usuario usuario)
    {
        var claimsList = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.UsuarioRoles
                .FirstOrDefault()?.Rol?.Nombre ?? "Cliente")
        };

        var permisos = usuario.UsuarioRoles
            .SelectMany(ur => ur.Rol!.RolPermisos)
            .Select(rp => rp.Permiso!.Nombre)
            .Distinct();

        foreach (var permiso in permisos)
            claimsList.Add(new Claim("permiso", permiso));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        int minutes = int.Parse(_configuration["Jwt:AccessTokenMinutes"]!);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claimsList,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerarRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private DateTime ObtenerExpiracionToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        return jwt.ValidTo;
    }

    private PerfilResponseDTO MapearPerfil(Usuario usuario)
    {
        return new PerfilResponseDTO
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Telefono = usuario.Telefono,
            Rol = usuario.UsuarioRoles.FirstOrDefault()?.Rol?.Nombre ?? "",
            FechaRegistro = usuario.FechaRegistro,
            UltimoLogin = usuario.UltimoLogin
        };
    }
}