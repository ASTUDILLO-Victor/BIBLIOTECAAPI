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
using OtpNet;

namespace BibliotecaApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenBlacklistRepository _blacklistRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IUsuarioRolRepository _usuarioRolRepository;
    private readonly IConfiguration _configuration;
    private readonly IQrCodeGenerator _qrCodeGenerator;

    public AuthService(
        IUnitOfWork unitOfWork,
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenBlacklistRepository blacklistRepository,
        IRolRepository rolRepository,
        IUsuarioRolRepository usuarioRolRepository,
         IQrCodeGenerator qrCodeGenerator,  // ← nuevo parámetro
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _blacklistRepository = blacklistRepository;
        _rolRepository = rolRepository;
        _usuarioRolRepository = usuarioRolRepository;
        _qrCodeGenerator = qrCodeGenerator;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
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

        _unitOfWork.BeginTransaction();
        try
        {
            var creado = _usuarioRepository.Agregar(usuario);

            var rolCliente = _rolRepository.ObtenerPorNombre("Lector");
            if (rolCliente == null)
                throw new KeyNotFoundException("Rol Lector no encontrado en BD");

            _usuarioRolRepository.Agregar(new UsuarioRol
            {
                UsuarioId = creado.Id,
                RolId = rolCliente.Id
            });
            _unitOfWork.Commit();

            var usuarioConRoles = _usuarioRepository.ObtenerPorId(creado.Id)!;
        

            // Debug temporal
            if (usuarioConRoles == null)
                throw new Exception("usuarioConRoles es null");
            if (usuarioConRoles.UsuarioRoles == null)
                throw new Exception("UsuarioRoles es null");
            if (usuarioConRoles.UsuarioRoles.Count == 0)
                throw new Exception("UsuarioRoles está vacío");
            if (usuarioConRoles.UsuarioRoles.First().Rol == null)
                throw new Exception("Rol es null dentro de UsuarioRoles");


            return GenerarAuthResponse(usuarioConRoles);
            
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public AuthResponseDTO Login(LoginConCodigoDTO dto)
    {
        var usuario = _usuarioRepository.ObtenerPorEmail(dto.Email);
        if (usuario == null)
            throw new UnauthorizedAccessException("Email o contraseña incorrectos");

        if (!usuario.Activo)
            throw new UnauthorizedAccessException("Usuario desactivado. Contacte al administrador");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            throw new UnauthorizedAccessException("Email o contraseña incorrectos");
        if (usuario.TwoFactorEnabled)
        {
            if (string.IsNullOrEmpty(dto.CodigoTwoFactor))
                throw new ArgumentException("Se requiere el código de autenticación de dos factores");

            bool codigoValido = VerificarCodigoTotp(usuario.TwoFactorSecret!, dto.CodigoTwoFactor);
            if (!codigoValido)
                throw new UnauthorizedAccessException("Código de autenticación incorrecto");
        }

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


    //2FA//
    public Habilitar2FAResponseDTO Habilitar2FA(int usuarioId)
    {
        var usuario = _usuarioRepository.ObtenerPorId(usuarioId);
        if (usuario == null)
            throw new KeyNotFoundException("Usuario no encontrado");

        // Generar un secreto único para este usuario
        var key = KeyGeneration.GenerateRandomKey(20);
        string secretoBase32 = Base32Encoding.ToString(key);

        // Guardar el secreto temporalmente (aún no activado)
        usuario.TwoFactorSecret = secretoBase32;
        _usuarioRepository.Actualizar(usuario);

        // Generar el QR para escanear con Google Authenticator
        string qrCodeBase64 = GenerarQrCode(usuario.Email, secretoBase32);

        return new Habilitar2FAResponseDTO
        {
            Secreto = secretoBase32,
            QrCodeBase64 = qrCodeBase64
        };
    }

    public void ConfirmarHabilitacion2FA(int usuarioId, Verificar2FADTO dto)
    {
        var usuario = _usuarioRepository.ObtenerPorId(usuarioId);
        if (usuario == null)
            throw new KeyNotFoundException("Usuario no encontrado");

        if (string.IsNullOrEmpty(usuario.TwoFactorSecret))
            throw new ArgumentException("Primero debes generar el secreto de 2FA");

        bool esValido = VerificarCodigoTotp(usuario.TwoFactorSecret, dto.Codigo);
        if (!esValido)
            throw new ArgumentException("Código incorrecto");

        usuario.TwoFactorEnabled = true;
        _usuarioRepository.Actualizar(usuario);
    }

    public AuthResponseDTO LoginCon2FA(LoginConCodigoDTO dto)
    {
        var usuario = _usuarioRepository.ObtenerPorEmail(dto.Email);
        if (usuario == null)
            throw new UnauthorizedAccessException("Email o contraseña incorrectos");

        if (!usuario.Activo)
            throw new UnauthorizedAccessException("Usuario desactivado");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            throw new UnauthorizedAccessException("Email o contraseña incorrectos");

        // Si el usuario tiene 2FA activado, verificar el código
        if (usuario.TwoFactorEnabled)
        {
            if (string.IsNullOrEmpty(dto.CodigoTwoFactor))
                throw new ArgumentException("Se requiere el código de autenticación de dos factores");

            bool codigoValido = VerificarCodigoTotp(usuario.TwoFactorSecret!, dto.CodigoTwoFactor);
            if (!codigoValido)
                throw new UnauthorizedAccessException("Código de autenticación incorrecto");
        }

        usuario.UltimoLogin = DateTime.UtcNow;
        _usuarioRepository.Actualizar(usuario);

        return GenerarAuthResponse(usuario);
    }

    public void Deshabilitar2FA(int usuarioId)
    {
        var usuario = _usuarioRepository.ObtenerPorId(usuarioId);
        if (usuario == null)
            throw new KeyNotFoundException("Usuario no encontrado");

        usuario.TwoFactorEnabled = false;
        usuario.TwoFactorSecret = null;
        _usuarioRepository.Actualizar(usuario);
    }

    // Métodos privados de apoyo
    private bool VerificarCodigoTotp(string secretoBase32, string codigo)
    {
        var key = Base32Encoding.ToBytes(secretoBase32);
        var totp = new Totp(key);
        return totp.VerifyTotp(codigo, out _, new VerificationWindow(1, 1));
    }

    private string GenerarQrCode(string email, string secretoBase32)
    {
        string issuer = "BibliotecaApp";
        string otpUri = $"otpauth://totp/{issuer}:{email}?secret={secretoBase32}&issuer={issuer}";
        return _qrCodeGenerator.GenerarQrBase64(otpUri);
    }

    public AuthResponseDTO LoginConGoogle(string email, string nombre)
    {
        var usuario = _usuarioRepository.ObtenerPorEmail(email);

        if (usuario == null)
        {
            // Usuario nuevo — crear cuenta automáticamente
            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Apellido = "",
                Email = email,
                Telefono = "",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), // contraseña aleatoria, nunca se usará
                Activo = true,
                FechaRegistro = DateTime.UtcNow
            };

            usuario = _usuarioRepository.Agregar(nuevoUsuario);

            var rolLector = _rolRepository.ObtenerPorNombre("Lector");
            if (rolLector == null)
                throw new KeyNotFoundException("Rol Lector no encontrado en BD");

            _usuarioRolRepository.Agregar(new UsuarioRol
            {
                UsuarioId = usuario.Id,
                RolId = rolLector.Id
            });

            usuario = _usuarioRepository.ObtenerPorId(usuario.Id)!;
        }

        if (!usuario.Activo)
            throw new UnauthorizedAccessException("Usuario desactivado");

        usuario.UltimoLogin = DateTime.UtcNow;
        _usuarioRepository.Actualizar(usuario);

        return GenerarAuthResponse(usuario);
    }
}