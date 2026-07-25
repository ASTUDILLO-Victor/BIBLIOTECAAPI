using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BibliotecaApp.Application.DTOs;
using BibliotecaApp.Application.Services.Interfaces;

namespace BibliotecaApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _service;

    public CategoriasController(ICategoriaService service)
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
    [Authorize(Policy = "gestionar_categorias")]
    public IActionResult Agregar([FromBody] CategoriaCreateDTO dto)
    {
        var categoriaCreada = _service.Agregar(dto);
        return CreatedAtAction(nameof(GetPorId), new { id = categoriaCreada.Id }, categoriaCreada);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "gestionar_categorias")]
    public IActionResult Actualizar(int id, [FromBody] CategoriaCreateDTO dto)
    {
        var categoriaActualizada = _service.Actualizar(id, dto);
        if (categoriaActualizada == null)
        {
            return NotFound();
        }
        return Ok(categoriaActualizada);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "gestionar_categorias")]
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