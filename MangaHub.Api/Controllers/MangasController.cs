using MangaHub.Api.Dtos;
using MangaHub.Api.Entities;
using MangaHub.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MangaHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class MangasController : ControllerBase
{
    private readonly IMangaRepository _mangas;
    private readonly ICatalogoRepository _catalogo;

    public MangasController(IMangaRepository mangas, ICatalogoRepository catalogo)
    {
        _mangas = mangas;
        _catalogo = catalogo;
    }

    /// <summary>Lista todos los mangas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MangaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MangaDto>>> GetAll()
    {
        var lista = await _mangas.ObtenerTodosAsync();
        return Ok(lista.Select(ToDto));
    }

    /// <summary>Obtiene un manga por su id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MangaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MangaDto>> GetById(int id)
    {
        var manga = await _mangas.ObtenerPorIdAsync(id);
        if (manga is null) return NotFoundProblem(id);

        return Ok(ToDto(manga));
    }

    /// <summary>Crea un manga nuevo.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(MangaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MangaDto>> Create(MangaRequestDto dto)
    {
        await ValidarReferenciasAsync(dto);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var manga = new Manga
        {
            Nombre = dto.Nombre.Trim(),
            Resumen = dto.Resumen,
            AutorId = dto.AutorId,
            CategoriaId = dto.CategoriaId
        };

        var creado = await _mangas.CrearAsync(manga);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, ToDto(creado));
    }

    /// <summary>Actualiza un manga existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, MangaRequestDto dto)
    {
        var manga = await _mangas.ObtenerPorIdAsync(id);
        if (manga is null) return NotFoundProblem(id);

        await ValidarReferenciasAsync(dto);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        manga.Nombre = dto.Nombre.Trim();
        manga.Resumen = dto.Resumen;
        manga.AutorId = dto.AutorId;
        manga.CategoriaId = dto.CategoriaId;

        await _mangas.ActualizarAsync(manga);
        return NoContent();
    }

    /// <summary>Elimina un manga.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var manga = await _mangas.ObtenerPorIdAsync(id);
        if (manga is null) return NotFoundProblem(id);

        await _mangas.EliminarAsync(manga);
        return NoContent();
    }

    private async Task ValidarReferenciasAsync(MangaRequestDto dto)
    {
        if (!await _catalogo.ExisteAutorAsync(dto.AutorId))
            ModelState.AddModelError(nameof(dto.AutorId), "El autor indicado no existe.");

        if (!await _catalogo.ExisteCategoriaAsync(dto.CategoriaId))
            ModelState.AddModelError(nameof(dto.CategoriaId), "La categoría indicada no existe.");
    }

    private ObjectResult NotFoundProblem(int id) =>
        Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Manga no encontrado",
            detail: $"No existe un manga con id {id}.");

    private static MangaDto ToDto(Manga m) =>
        new(m.Id, m.Nombre, m.Resumen,
            m.AutorId, m.Autor?.Nombre ?? string.Empty,
            m.CategoriaId, m.Categoria?.Nombre ?? string.Empty);
}
