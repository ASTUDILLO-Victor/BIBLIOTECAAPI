using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BibliotecaApp.Application.DTOs;
using BibliotecaApp.Application.Services.Interfaces;

namespace BibliotecaApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamosController : ControllerBase
{
    private readonly IPrestamoService _service;

    public PrestamosController(IPrestamoService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "ver_prestamos")]
    public IActionResult GetTodos()
    {
        return Ok(_service.ObtenerTodos());
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ver_prestamos")]
    public IActionResult GetPorId(int id)
    {
        var prestamo = _service.ObtenerPorId(id);
        if (prestamo == null)
            return NotFound($"No existe préstamo con Id {id}");
        return Ok(prestamo);
    }

    [HttpGet("mis-prestamos")]
    [Authorize]
    public IActionResult GetMisPrestamos()
    {
        int usuarioId = ObtenerUsuarioId();
        return Ok(_service.ObtenerPorUsuario(usuarioId));
    }

    [HttpPost]
    [Authorize]
    public IActionResult Agregar([FromBody] PrestamoCreateDTO dto)
    {
        int usuarioId = ObtenerUsuarioId();
        var creado = _service.Agregar(dto, usuarioId);
        return CreatedAtAction(nameof(GetPorId), new { id = creado.Id }, creado);
    }

    [HttpPut("{id}/devolver")]
    [Authorize]
    public async Task<IActionResult> Devolver(int id)
    {
        var prestamo = await _service.Devolver(id);
        if (prestamo == null)
            return NotFound($"No existe préstamo con Id {id}");
        return Ok(prestamo);
    }

    private int ObtenerUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            throw new UnauthorizedAccessException("Usuario no autenticado");
        return int.Parse(claim.Value);
    }
}