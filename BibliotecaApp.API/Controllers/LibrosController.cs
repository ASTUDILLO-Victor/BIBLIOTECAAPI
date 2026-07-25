using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibliotecaApp.Application.DTOs;
using BibliotecaApp.Application.Services.Interfaces;

namespace BibliotecaApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibrosController : ControllerBase
{
    private readonly ILibroService _service;

    public LibrosController(ILibroService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetTodos()
    {
        return Ok(await _service.ObtenerTodos());
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult GetPorId(int id)
    {
        var libro = _service.ObtenerPorId(id);
        if (libro == null)
            return NotFound($"No existe libro con Id {id}");
        return Ok(libro);
    }

    // [HttpGet("categoria/{categoriaId}")]
    // [AllowAnonymous]
    // public IActionResult GetPorCategoria(int categoriaId)
    // {
    //     return Ok(_service.ObtenerPorCategoria(categoriaId));
    // }

    // [HttpGet("autor/{autorId}")]
    // [AllowAnonymous]
    // public IActionResult GetPorAutor(int autorId)
    // {
    //     return Ok(_service.ObtenerPorAutor(autorId));
    // }

    [HttpPost]
    [Authorize(Policy = "gestionar_libros")]
    public async Task<IActionResult>Agregar([FromBody] LibroCreateDTO dto)
    {
        var creado = await _service.Agregar(dto);
        return CreatedAtAction(nameof(GetPorId), new { id = creado.Id }, creado);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "gestionar_libros")]
    public IActionResult Actualizar(int id, [FromBody] LibroCreateDTO dto)
    {
        var actualizado = _service.Actualizar(id, dto);
        if (actualizado == null)
            return NotFound($"No existe libro con Id {id}");
        return Ok(actualizado);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "gestionar_libros")]
    public IActionResult Eliminar(int id)
    {
        bool eliminado = _service.Eliminar(id);
        if (!eliminado)
            return NotFound($"No existe libro con Id {id}");
        return NoContent();
    }
}