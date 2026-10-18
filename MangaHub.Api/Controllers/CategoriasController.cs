using MangaHub.Api.Dtos;
using MangaHub.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MangaHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CategoriasController : ControllerBase
{
    private readonly ICatalogoRepository _catalogo;

    public CategoriasController(ICatalogoRepository catalogo)
    {
        _catalogo = catalogo;
    }

    /// <summary>Lista las categorías.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAll()
    {
        var categorias = await _catalogo.ObtenerCategoriasAsync();
        return Ok(categorias.Select(c => new CategoriaDto(c.Id, c.Nombre, c.Descripcion)));
    }
}
