using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BibliotecaApp.Application.DTOs;
using BibliotecaApp.Application.Services.Interfaces;

namespace BibliotecaApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutoresController : ControllerBase
{
    private readonly IAutorService _service;

    public AutoresController(IAutorService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetTodos()
    {
        var autores = _service.ObtenerTodos();
        return Ok(autores);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult GetPorId(int id)
    {
        var autor = _service.ObtenerPorId(id);
        if (autor == null)
        {
            return NotFound();
        }
        return Ok(autor);
    }

    [HttpPost]
    [Authorize(Policy = "gestionar_autores")]
    public IActionResult Agregar([FromBody] AutorCreateDTO dto)
    {
        var autorCreado = _service.Agregar(dto);
        return CreatedAtAction(nameof(GetPorId), new { id = autorCreado.Id }, autorCreado);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "gestionar_autores")]
    public IActionResult Actualizar(int id, [FromBody] AutorCreateDTO dto)
    {
        var autorActualizado = _service.Actualizar(id, dto);
        if (autorActualizado == null)
        {
            return NotFound();
        }
        return Ok(autorActualizado);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "gestionar_autores")]
    public IActionResult Eliminar(int id)
    {
        var eliminado = _service.Eliminar(id);
        if (!eliminado)
        {
            return NotFound();
        }
        return NoContent();
    }
}