using Asp.Versioning;
using MangaHub.Api.Dtos;
using MangaHub.Api.Entities;
using MangaHub.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MangaHub.Api.Controllers;

/// <summary>Biblioteca personal: cada usuario solo ve y modifica sus propios mangas.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class BibliotecaController : ControllerBase
{
    private readonly IBibliotecaRepository _biblioteca;
    private readonly IMangaRepository _mangas;

    public BibliotecaController(IBibliotecaRepository biblioteca, IMangaRepository mangas)
    {
        _biblioteca = biblioteca;
        _mangas = mangas;
    }

    // el id sale del token (claim "sub"), no de lo que mande el cliente
    private int UsuarioId => int.Parse(User.FindFirst("sub")!.Value);

    /// <summary>Lista los mangas de mi biblioteca.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EntradaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EntradaDto>>> GetMiBiblioteca()
    {
        var entradas = await _biblioteca.ObtenerPorUsuarioAsync(UsuarioId);
        return Ok(entradas.Select(ToDto));
    }

    /// <summary>Obtiene un manga de mi biblioteca.</summary>
    [HttpGet("{mangaId:int}")]
    [ProducesResponseType(typeof(EntradaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntradaDto>> GetByManga(int mangaId)
    {
        var entrada = await _biblioteca.ObtenerAsync(UsuarioId, mangaId);
        if (entrada is null) return NotFoundProblem(mangaId);

        return Ok(ToDto(entrada));
    }

    /// <summary>Agrega un manga a mi biblioteca.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(EntradaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EntradaDto>> Agregar(EntradaRequestDto dto)
    {
        var manga = await _mangas.ObtenerPorIdAsync(dto.MangaId);
        if (manga is null)
        {
            ModelState.AddModelError(nameof(dto.MangaId), "El manga indicado no existe.");
            return ValidationProblem(ModelState);
        }

        if (await _biblioteca.ObtenerAsync(UsuarioId, dto.MangaId) is not null)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Manga ya agregado",
                detail: "Ese manga ya está en tu biblioteca.");
        }

        var entrada = new EntradaBiblioteca
        {
            UsuarioId = UsuarioId,
            MangaId = dto.MangaId,
            Estado = dto.Estado,
            CapituloActual = dto.CapituloActual
        };

        var creada = await _biblioteca.CrearAsync(entrada);
        return CreatedAtAction(nameof(GetByManga), new { mangaId = creada.MangaId }, ToDto(creada));
    }

    /// <summary>Cambia el estado de lectura y el capítulo actual de un manga de mi biblioteca.</summary>
    [HttpPut("{mangaId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(int mangaId, EntradaUpdateDto dto)
    {
        var entrada = await _biblioteca.ObtenerAsync(UsuarioId, mangaId);
        if (entrada is null) return NotFoundProblem(mangaId);

        entrada.Estado = dto.Estado;
        entrada.CapituloActual = dto.CapituloActual;

        await _biblioteca.ActualizarAsync(entrada);
        return NoContent();
    }

    /// <summary>Quita un manga de mi biblioteca.</summary>
    [HttpDelete("{mangaId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Quitar(int mangaId)
    {
        var entrada = await _biblioteca.ObtenerAsync(UsuarioId, mangaId);
        if (entrada is null) return NotFoundProblem(mangaId);

        await _biblioteca.EliminarAsync(entrada);
        return NoContent();
    }

    private ObjectResult NotFoundProblem(int mangaId) =>
        Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Manga no encontrado en tu biblioteca",
            detail: $"El manga {mangaId} no está en tu biblioteca.");

    private static EntradaDto ToDto(EntradaBiblioteca e) =>
        new(e.MangaId, e.Manga?.Nombre ?? string.Empty, e.Estado, e.CapituloActual);
}
