using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BibliotecaApp.Application.DTOs;
using BibliotecaApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace BibliotecaApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("registro")]
    public IActionResult Registro([FromBody] RegistroDTO dto)
    {
        var resultado = _service.Registro(dto);
        return Ok(resultado);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginConCodigoDTO dto)
    {
        var resultado = _service.Login(dto);
        return Ok(resultado);
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout([FromBody] RefreshTokenDTO dto)
    {
        string accessToken = Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");
        _service.Logout(accessToken, dto.RefreshToken);
        return Ok("Sesión cerrada correctamente");
    }

    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshTokenDTO dto)
    {
        var resultado = _service.Refresh(dto.RefreshToken);
        return Ok(resultado);
    }

    [HttpGet("perfil")]
    [Authorize]
    public IActionResult ObtenerPerfil()
    {
        int usuarioId = ObtenerUsuarioId();
        var perfil = _service.ObtenerPerfil(usuarioId);
        return Ok(perfil);
    }

    private int ObtenerUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            throw new UnauthorizedAccessException("Usuario no autenticado");
        return int.Parse(claim.Value);
    }

    [HttpPost("2fa/habilitar")]
    [Authorize]
    public IActionResult Habilitar2FA()
    {
        int usuarioId = ObtenerUsuarioId();
        var resultado = _service.Habilitar2FA(usuarioId);
        return Ok(resultado);
    }

    [HttpPost("2fa/confirmar")]
    [Authorize]
    public IActionResult ConfirmarHabilitacion2FA([FromBody] Verificar2FADTO dto)
    {
        int usuarioId = ObtenerUsuarioId();
        _service.ConfirmarHabilitacion2FA(usuarioId, dto);
        return Ok("2FA activado correctamente");
    }

    [HttpPost("2fa/deshabilitar")]
    [Authorize]
    public IActionResult Deshabilitar2FA()
    {
        int usuarioId = ObtenerUsuarioId();
        _service.Deshabilitar2FA(usuarioId);
        return Ok("2FA desactivado correctamente");
    }

    [HttpPost("login-2fa")]
    public IActionResult LoginCon2FA([FromBody] LoginConCodigoDTO dto)
    {
        var resultado = _service.LoginCon2FA(dto);
        return Ok(resultado);
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback));
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync("GoogleTemp");

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                error = result.Failure?.Message,
                innerError = result.Failure?.InnerException?.Message
            });
        }

        var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;
        var nombre = result.Principal?.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(email))
            return BadRequest("Google no devolvió un email válido");

        var authResponse = _service.LoginConGoogle(email, nombre ?? "Usuario Google");
        return Ok(authResponse);
    }
}